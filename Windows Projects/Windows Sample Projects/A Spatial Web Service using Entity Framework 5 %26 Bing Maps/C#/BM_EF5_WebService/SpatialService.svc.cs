using BM_EF5_WebService.Common;
using Microsoft.SqlServer.Types;
using System;
using System.Data.Spatial;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Activation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BM_EF5_WebService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SpatialService : ISpatialService
    {
        private Regex shortenCoordinate = new Regex("([0-9].[0-9]{5})[0-9]*");

        public Response FindNearBy(double latitude, double longitude, double radius, string layerName)
        {
            Response r = new Response();

            try
            {
                DbGeography center = DbGeography.PointFromText("POINT(" + longitude + " " + latitude + ")", 4326);

                using (SpatialSampleEntities e = new SpatialSampleEntities())
                {
                    switch (layerName.ToLowerInvariant())
                    {
                        case "cities":
                            r.Results = (from c in e.Cities
                                         let distance = center.Distance(c.Location)
                                         where distance <= radius
                                         orderby distance
                                         select new Common.City()
                                         {
                                            Name = c.Name,
                                            CountryISO = c.Country_ISO,
                                            Population = c.Population.HasValue ? (int)c.Population.Value : 0,
                                            WKT = c.Location.AsText(),
                                            Distance = (distance.HasValue) ? Math.Round(distance.Value) : 0
                                         }).ToList<BaseEntity>();
                            break;
                        case "countries":
                            r.Results = (from c in e.Countries
                                         let distance = center.Distance(c.Boundary)
                                         where distance <= radius
                                         orderby distance
                                         select new Common.Country()
                                         {
                                             ISO = c.ISO,
                                             Population = c.Population.HasValue ? (int)c.Population.Value : 0,
                                             Name = c.Name,
                                             WKT = c.Boundary.AsText(),
                                             Distance = (distance.HasValue) ? Math.Round(distance.Value) : 0
                                         }).ToList<BaseEntity>();
                            break;
                        default:
                            r.Error = "Invalid Layer Name.";
                            break;
                    }

                    if (r.Results != null)
                    {
                        r.Results.ForEach(c => c.WKT = shortenCoordinate.Replace(c.WKT, "$1"));
                    }
                }
            }
            catch(Exception ex){
                r.Error = ex.Message;
            }

            return r;       
        }

        public Response FindInPolygon(string polygonWKT, string layerName)
        {
            Response r = new Response();

            try
            {
                DbGeography polygon = DbGeography.FromText(polygonWKT, 4326);
                
                if (polygon.Area.HasValue && polygon.Area.Value > (580000000000000 / 2)){
                    SqlGeography sqlPolygon = SqlGeography.STGeomFromWKB(new System.Data.SqlTypes.SqlBytes(polygon.AsBinary()), 4326);
                    sqlPolygon = sqlPolygon.ReorientObject();
                    polygon = DbGeography.FromBinary(sqlPolygon.STAsBinary().Value);
                }

                using (SpatialSampleEntities e = new SpatialSampleEntities())
                {
                    switch (layerName.ToLowerInvariant())
                    {
                        case "cities":
                            r.Results = (from c in e.Cities
                                        where c.Location.Intersects(polygon)
                                        select new Common.City()
                                        {
                                            Name = c.Name,
                                            CountryISO = c.Country_ISO,
                                            Population = c.Population.HasValue ? (int)c.Population.Value : 0,
                                            WKT = c.Location.AsText()
                                        }).ToList<BaseEntity>();
                            break;
                        case "countries":
                            r.Results = (from c in e.Countries
                                            where c.Boundary.Intersects(polygon)
                                            select new Common.Country()
                                            {
                                                ISO = c.ISO,
                                                Population = c.Population.HasValue ? (int)c.Population.Value : 0,
                                                Name = c.Name,
                                                WKT = c.Boundary.AsText()
                                            }).ToList<BaseEntity>();
                            break;
                        default:
                            r.Error = "Invalid Layer Name.";
                            break;
                    }

                    if (r.Results != null)
                    {
                        r.Results.ForEach(c => c.WKT = shortenCoordinate.Replace(c.WKT, "$1"));
                    }
                }
            }
            catch (Exception ex)
            {
                r.Error = ex.Message;
            }

            return r;
        }

        public Response FindByBoundingBox(double north, double east, double south, double west, string layerName)
        {
            //Create poylgon of bounding box
            string bboxWKT = string.Format("POLYGON(({0} {1},{2} {1},{2} {3},{0} {3},{0} {1}))", east, north, west, south);
            return FindInPolygon(bboxWKT, layerName);
        }

        public Response FindNearRoute(string waypoints, double radius, string bingMapsKey, string layerName)
        {
            Response response = new Response();

            try
            {
                //Create the request URL for the route service
                string[] wp = waypoints.Split(new char[]{'|'});

                if (wp.Length < 2)
                {
                    throw new Exception("Invalid number of waypoints.");
                }

                StringBuilder request = new StringBuilder("http://dev.virtualearth.net/REST/V1/Routes/Driving?rpo=Points");

                for (int i = 0; i < wp.Length; i++)
                {
                    request.AppendFormat("&wp.{0}={1}", i, wp[i]);
                }

                request.AppendFormat("&key={0}", bingMapsKey);

                Uri routeRequest = new Uri(request.ToString());

                BingMapsRESTService.Common.JSON.Response r = GetResponse(routeRequest);

                if (r == null ||
                r.ResourceSets == null ||
                r.ResourceSets.Length == 0 ||
                r.ResourceSets[0].Resources == null ||
                r.ResourceSets[0].Resources.Length == 0)
                {
                    throw new Exception("Unable to calculate route between waypoints.");
                }
                    
                BingMapsRESTService.Common.JSON.Route route = (BingMapsRESTService.Common.JSON.Route)r.ResourceSets[0].Resources[0];

                if(route.RoutePath == null ||
                route.RoutePath.Line == null ||
                route.RoutePath.Line.Coordinates == null ||
                route.RoutePath.Line.Coordinates.Length == 0)
                {
                    throw new Exception("Unable to calculate route between waypoints.");
                }

                var coords = route.RoutePath.Line.Coordinates;

                //Turn the coordinate array into an SQLGeography object
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(4326);

                builder.BeginGeography(OpenGisGeographyType.LineString);
                builder.BeginFigure(coords[0][0], coords[0][1]);
                
                for (var i = 1; i < coords.Length; i++)
                {
                    if (coords[i].Length >= 2)
                    {
                        builder.AddLine(coords[i][0], coords[i][1]);
                    }
                }

                builder.EndFigure();
                builder.EndGeography();

                //Reduce the resolution of the line and give is a buffer equal to our radius
                var buffer = builder.ConstructedGeography.Reduce(100);
                buffer = buffer.BufferWithTolerance(radius, 100, false);

                //Turn the SqlGeography object into a DbGeography object
                DbGeography routeBuffer = DbGeography.FromBinary(buffer.STAsBinary().Value, 4326);

                using (SpatialSampleEntities e = new SpatialSampleEntities())
                {
                    switch (layerName.ToLowerInvariant())
                    {
                        case "cities":
                            response.Results = (from c in e.Cities
                                            where c.Location.Intersects(routeBuffer)
                                            select new Common.City()
                                            {
                                                Name = c.Name,
                                                CountryISO = c.Country_ISO,
                                                Population = c.Population.HasValue ? (int)c.Population.Value : 0,
                                                WKT = c.Location.AsText()
                                            }).ToList<BaseEntity>();
                            break;
                        case "countries":
                            response.Results = (from c in e.Countries
                                            where c.Boundary.Intersects(routeBuffer)
                                            select new Common.Country()
                                            {
                                                ISO = c.ISO,
                                                Population = c.Population.HasValue ? (int)c.Population.Value : 0,
                                                Name = c.Name,
                                                WKT = c.Boundary.AsText()
                                            }).ToList<BaseEntity>();
                            break;
                        default:
                            response.Error = "Invalid Layer Name.";
                            break;
                    }

                    if (response.Results != null)
                    {
                        response.Results.ForEach(c => c.WKT = shortenCoordinate.Replace(c.WKT, "$1"));
                    }
                }
            }
            catch(Exception ex){
                response.Error = ex.Message;
            }

            return response;   
        }

        #region Private Methods

        private BingMapsRESTService.Common.JSON.Response GetResponse(Uri uri)
        {
            WebClient client = new WebClient();
            using (var stream = client.OpenRead(uri))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BingMapsRESTService.Common.JSON.Response));
                return ser.ReadObject(stream) as BingMapsRESTService.Common.JSON.Response;
            }
        }

        #endregion
    }
}

using BM_EF5_WebService.Common;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BM_EF5_WebService
{
    [ServiceContract(Namespace = "SpatialService")]
    [ServiceKnownType(typeof(Response))]
    public interface ISpatialService
    {
        /// <summary>
        /// Finds all locations that are within a specified distance of a central coordinate for a specified layer.
        /// </summary>
        /// <param name="latitude">Center latitude value.</param>
        /// <param name="longitude">Center longitude value.</param>
        /// <param name="radius">Search radius in Meters</param>
        /// <param name="layerName">Name of the layer (SQL table) to search against.</param>
        /// <returns>A list of results</returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Response FindNearBy(double latitude, double longitude, double radius, string layerName);

        /// <summary>
        /// Finds all locations that are within a polygon for a specified layer.
        /// </summary>
        /// <param name="polygonWKT">Well Known Text for a Polygon.</param>
        /// <param name="layerName">Name of the layer (SQL table) to search against.</param>
        /// <returns>A list of results</returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Response FindInPolygon(string polygonWKT, string layerName);

        /// <summary>
        /// Find all locations that are within a bounding box for a specified layer.
        /// </summary>
        /// <param name="north">North Latitude</param>
        /// <param name="east">East Longitude</param>
        /// <param name="south">South Latitude</param>
        /// <param name="west">West Longitude</param>
        /// <param name="layerName">Name of the layer (SQL table) to search against.</param>
        /// <returns>A list of results</returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Response FindByBoundingBox(double north, double east, double south, double west, string layerName);

        /// <summary>
        /// Finds all locations that are within a specified distance (radius) of a route path for a specified layer.
        /// </summary>
        /// <param name="waypoints">A pipe (|) delimited list of waypoints a route goes through.</param>
        /// <param name="radius">Search radius aourd the route in meters.</param>
        /// <param name="bingMapsKey">Bing Maps key for requesting route.</param>
        /// <param name="layerName">Name of the layer (SQL table) to search against.</param>
        /// <returns>A list of results</returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Response FindNearRoute(string waypoints, double radius, string bingMapsKey, string layerName);
    }
}

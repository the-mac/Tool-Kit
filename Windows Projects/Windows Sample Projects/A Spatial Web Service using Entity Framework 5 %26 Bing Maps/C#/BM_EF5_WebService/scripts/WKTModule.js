/****************************************************************************
* Author: Ricky Brundritt
* Website: http://rbrundritt.wordpress.com
* Date: May 23rd, 2012
*
* Description:
* This module allows you to easily read and write Well Know Text data. 
* Well Know Text is read into Bing Maps shapes. Note that MultiPoint, 
* MultiLinstring, and GeometryCollection are turned into an EntityCollection 
* of shapes.
*
* Supported WKT tags:
*   - Point
*   - Linestring
*   - Polygon
*   - MultiPoint
*   - MultiLinestring
*   - MultiPolygon
*   - Geometrycollection
*
* Loading the Module:
* -------------------
* //Register and load the Persistence Module
* Microsoft.Maps.registerModule("WKTModule", "scripts/WKTModule.js");
* Microsoft.Maps.loadModule("WKTModule");
*
* Write Example:
* --------------
* var p = new Microsoft.Maps.Point(new Microsoft.Maps.Location(0,0));
* var wkt = WKTModule.Write(p);
* --> wkt = 'POINT(0,0)'
*
* Read Example:
* -------------
* var wkt = 'LINESTRING(0 0, 10 10)';
* var shape = WKTModule.Read(wkt);
* --> shape = Microsoft.Map.Polyline
*
* Options:
* --------
*
* Style Options - Pass in styles into the Read method to override the default 
* styles options for generating the shapes. These styles will be merged with 
* the default option styles.
*
****************************************************************************/

var WKTModule = new function () {
    var _options = {
        pushpinOptions: { /* Pushpin Options */ },
        polylineOptions: {
            strokeColor: new Microsoft.Maps.Color(200, 0, 255, 0),
            strokeThickness: 2
        },
        polygonOptions: {
            fillColor: new Microsoft.Maps.Color(100, 0, 0, 255),
            strokeColor: new Microsoft.Maps.Color(200, 255, 0, 0),
            strokeThickness: 2
        }
    };

    /********************
    * Private Methods
    *********************/

    function mergeStyles(styles) {
        var t = _options;
        for (attrname in styles) {
            t[attrname] = styles[attrname];
        }

        return t;
    }

    /********************
    * Parse Methods
    *********************/

    function ParseCoordinate(wkt) {
        var coord = wkt.replace(/(^\s|\s+$)/gi, '').split(/\s+/);

        if (coord.length > 1) {
            return new Microsoft.Maps.Location(parseFloat(coord[1]), parseFloat(coord[0]));
        }

        return null;
    }

    function ParseRing(wkt) {
        var coordPairs = wkt.replace(/(^\s|\s+$)/gi, '').split(/,\s*/);

        var coords = [], coord;
        for (var i = 0; i < coordPairs.length; i++) {
            coord = ParseCoordinate(coordPairs[i]);

            if (coord != null) {
                coords.push(coord);
            }
        }

        return coords;
    }

    function ParsePoint(wkt, styles) {
        wkt = wkt.replace(/(POINT|[\(\)]*)/gi, '');
        var c = ParseCoordinate(wkt);
        if (c != null) {
            return new Microsoft.Maps.Pushpin(c, styles.pushpinOptions);
        }
    }

    function ParseLinestring(wkt, styles) {
        wkt = wkt.replace(/(LINESTRING|[\(\)]*|^\s*|\s$)/gi, '');
        return new Microsoft.Maps.Polyline(ParseRing(wkt), styles.polylineOptions);
    }

    function ParsePolygon(wkt, styles) {
        var wktRings = wkt.replace(/(POLYGON|\(\(|\)\))/gi, '').split(/\),\s*\(/);
        var rings = [];

        for (var i in wktRings) {
            rings.push(ParseRing(wktRings[i]));
        }

        return new Microsoft.Maps.Polygon(rings, styles.polygonOptions);
    }

    function ParseMultiPoint(wkt, styles) {
        var col = new Microsoft.Maps.EntityCollection(), c;
        var points = wkt.replace(/(MULTIPOINT|[\(\)]*)/gi, '').split(/,\s*/);

        for (var i in points) {
            c = ParseCoordinate(points[i]);
            if (c != null) {
                col.push(new Microsoft.Maps.Pushpin(c, styles.pushpinOptions));
            }
        }

        return col;
    }

    function ParseMultiLinestring(wkt, styles) {
        var col = new Microsoft.Maps.EntityCollection();
        var wktRings = wkt.replace(/(MULTILINESTRING|\(\(|\)\))/gi, '').split(/\),\s*\(/);

        for (var i in wktRings) {
            col.push(new Microsoft.Maps.Polyline(ParseRing(wktRings[i]), styles.polylineOptions));
        }

        return col;
    }

    function ParseMultiPolygon(wkt, styles) {
        var col = new Microsoft.Maps.EntityCollection();
        var wktPolys = wkt.replace(/(MULTIPOLYGON|\(\(\(|\)\)\))/gi, '').split(/\)\),\s*\(\(/);
        var rings, wktRings;

        for (var i in wktPolys) {
            rings = [];
            wktRings = wktPolys[i].split(/\),\s*\(/);

            for (var r in wktRings) {
                rings.push(ParseRing(wktRings[r]));
            }

            col.push(new Microsoft.Maps.Polygon(rings, styles.polygonOptions));
        }

        return col;
    }

    function ParseGeometryCollection(wkt, styles) {
        var col = new Microsoft.Maps.EntityCollection(), shape;
        var wktShapes = wkt.replace(/(GEOMETRYCOLLECTION\s*\(|(\)\s*)$)/gi, '').replace(/(\))(,\s*)([a-zA-Z])/gi, '$1|$3').split('|');

        for (var i in wktShapes) {
            shape = ParseShape(wktShapes[i], styles);

            if (shape != null) {
                col.push(shape);
            }
        }

        return col;
    }

    function ParseShape(wkt, styles) {
        if (wkt) {
            var name = wkt.substring(0, wkt.indexOf('(', 0));
            name = name.replace(/\s/g, '');

            switch (name.toLowerCase()) {
                case 'point':
                    return ParsePoint(wkt, styles);
                case 'linestring':
                    return ParseLinestring(wkt, styles);
                case 'polygon':
                    return ParsePolygon(wkt, styles);
                case 'multipoint':
                    return ParseMultiPoint(wkt, styles);
                case 'multilinestring':
                    return ParseMultiLinestring(wkt, styles);
                case 'multipolygon':
                    return ParseMultiPolygon(wkt, styles);
                case 'geometrycollection':
                    return ParseGeometryCollection(wkt, styles);
                default:
                    break;
            }
        }

        return null;
    }

    /********************
    * Write Methods
    *********************/

    function WriteRing(locs) {
        var ring = ['('];
        var len = locs.length;

        for (var i = 0; i < len; i++) {
            ring.push(Math.round(locs[i].longitude * 100000) / 100000, ' ', Math.round(locs[i].latitude * 100000) / 100000);

            if (i < len - 1) {
                ring.push(',');
            }
        }

        ring.push(')');

        return ring.join('');
    }

    function WritePoint(point) {
        var p = point.getLocation();
        return 'POINT(' + Math.round(p.longitude * 100000) / 100000 + ' ' + Math.round(p.latitude * 100000) / 100000 + ')';
    }

    function WriteLinestring(polyline) {
        return 'LINESTRING' + WriteRing(polyline.getLocations());
    }

    function WritePolygon(polygon) {
        var wkt = ['POLYGON('];

        if (polygon.getRings) {
            var rings = polygon.getRings();
            var numRings = rings.length;

            for (var i = 0; i < numRings; i++) {
                wkt.push(WriteRing(rings[i]));

                if (i < numRings - 1) {
                    wkt.push(',');
                }
            }
        } else {
            wkt.push(WriteRing(polygon.getLocations()));
        }

        wkt.push(')');

        return wkt.join('');
    }

    function WriteGeometryCollection(collection) {
        var numShapes = collection.getLength(), shape, 
            wkt = ['GEOMETRYCOLLECTION('], temp,
            numPoints = 0, numLines = 0, numPolys = 0;

        for (var i = 0; i < numShapes; i++) {
            shape = collection.get(i);
            if (shape.getFillColor) {           //Only Polygons have fill color
                wkt.push(WritePolygon(shape), ',');
                numPolys++;
            } else if (shape.getIcon) {         //Only Pushpins have an icon
                wkt.push(WritePoint(shape), ',');
                numPoints++;
            } else if (shape.getStrokeColor) {  //Only Polylines and Polygons have this property. Polygons would of alreayd been caught above.
                wkt.push(WriteLinestring(shape), ',');
                numLines++;
            }
        }

        temp = wkt.join('').replace(/,+$/g, '') + ')';

        //Check to see if collection is multi geometry
        if (numPoints > 0 && numLines == 0 && numPolys == 0) {
            temp = temp.replace(/POINT/gi, '').replace(/GEOMETRYCOLLECTION/gi, 'MULTIPOINT');
        } else if (numPoints == 0 && numLines > 0 && numPolys == 0) {
            temp = temp.replace(/LINESTRING/gi, '').replace(/GEOMETRYCOLLECTION/gi, 'MULTILINESTRING');
        } else if (numPoints == 0 && numLines == 0 && numPolys > 0) {
            temp = temp.replace(/POLYGON/gi, '').replace(/GEOMETRYCOLLECTION/gi, 'MULTIPOLYGON');
        }

        return temp;
    }

    //Allow ignoring collections to prevent embedded collections from creating GeomCollections inside of GeomCollections.
    function WriteShape(shape) {
        if (shape.getFillColor) {           //Only Polygons have fill color
            return WritePolygon(shape);
        } else if (shape.getIcon) {         //Only Pushpins have an icon
            return WritePoint(shape);
        } else if (shape.getStrokeColor) {  //Only Polylines and Polygons have this property. Polygons would of alreayd been caught above.
            return WriteLinestring(shape);
        } else if (shape.getLength) { //Only EntityCollections have getLength property.
            return WriteGeometryCollection(shape);
        }

        //Ignore all other shapes such as infobox and tile layers.
        return null;
    }

    /********************
    * Public Methods
    *********************/

    this.Read = function (wkt, styles) {
        //Merge new styles with default styles
        return ParseShape(wkt, mergeStyles(styles));
    };

    this.Write = function (shape) {
        return WriteShape(shape);
    };
};

(function () {
    //Load complex polygon module is not already loaded
    var p = new Microsoft.Maps.Polygon();
    if (!p.getRings) {
        Microsoft.Maps.loadModule('Microsoft.Maps.AdvancedShapes', { callback: function(){
            // Call the Module Loaded method
            Microsoft.Maps.moduleLoaded('WKTModule');
        }});
    }else{
        // Call the Module Loaded method
        Microsoft.Maps.moduleLoaded('WKTModule');
    }
})();
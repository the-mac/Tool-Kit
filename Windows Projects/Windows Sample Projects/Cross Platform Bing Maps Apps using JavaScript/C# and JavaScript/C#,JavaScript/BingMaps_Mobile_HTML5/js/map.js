(function () {
    var map,
        searchManager,
        geoLocationProvider,
        gpsLayer,
        gpsEnabled = false,
        trafficLayer,
        trafficEnabled = false;

    function init() {
        //Check to see if the Windows 8 version of the map control is being used by checking for the ClientRegion property on the map control.
        if (Microsoft.Maps.ClientRegion) {
            Microsoft.Maps.loadModule('Microsoft.Maps.Map', { callback: getMap });
        } else {
            getMap();
        }

        //Test for multi-touch support
        if ('ongesturechange' in window || //iOS & android
            (navigator.msMaxTouchPoints && navigator.msMaxTouchPoints > 1)) //Win8, WP8 & IE10
        {
            //Hide zoom and rotate controls
            document.getElementById('zoomRotateBar').style.display = 'none';
        } else {
            //Only display rotation buttons when the map style is birdseye
            Microsoft.Maps.Events.addHandler(map, 'maptypechanged', updateNavBar);
            Microsoft.Maps.Events.addHandler(map, 'viewchangeend', updateNavBar);

            //Add zooming and rotating functionality
            addListener(document.getElementById('zoomInBtn'), 'click', function () {
                map.setView({ zoom: map.getZoom() + 1 });
            });

            addListener(document.getElementById('zoomOutBtn'), 'click', function () {
                map.setView({ zoom: map.getZoom() - 1 });
            });

            addListener(document.getElementById('rotateCWBtn'), 'click', function () {
                map.setView({ heading: map.getHeading() + 90 });
            });

            addListener(document.getElementById('rotateCCWBtn'), 'click', function () {
                map.setView({ heading: map.getHeading() - 90 });
            });
        }

        //Test for single-touch support
        if ('ontouchstart' in window || //iOS & android
            (navigator.msMaxTouchPoints && navigator.msMaxTouchPoints > 0)) //Win8, WP8 & IE10
        {
            //Hide navigation controls
            document.getElementById('navBar').style.display = 'none';
        } else {
            //Add panning functionality
            addListener(document.getElementById('upBtn'), 'click', function () {
                map.setView({ center: map.getCenter(), centerOffset: new Microsoft.Maps.Point(0, 100) });
            });

            addListener(document.getElementById('leftBtn'), 'click', function () {
                map.setView({ center: map.getCenter(), centerOffset: new Microsoft.Maps.Point(100, 0) });
            });

            addListener(document.getElementById('rightBtn'), 'click', function () {
                map.setView({ center: map.getCenter(), centerOffset: new Microsoft.Maps.Point(-100, 0) });
            });

            addListener(document.getElementById('downBtn'), 'click', function () {
                map.setView({ center: map.getCenter(), centerOffset: new Microsoft.Maps.Point(0, -100) });
            });
        }

        addListener(document.getElementById('searchBtn'), 'click', function () {
            document.getElementById('searchPanel').style.display = '';
            document.getElementById('searchTbx').focus();
        });

        addListener(document.getElementById('searchTbx'), 'keydown', function (e) {
            if (!e) {
                e = window.event;
            }

            //process search when enter key pressed
            if (e.keyCode == 13) {
                search(this.value);
            }
        });

        addListener(document.getElementById('gpsBtn'), 'click', toggleGPS);

        addListener(document.getElementById('trafficBtn'), 'click', toggleTraffic);

        addListener(document.getElementById('mapModeBtn'), 'click', function () {
            document.getElementById('mapModePanel').style.display = '';
        });

        var mapModeBtns = document.getElementsByName('mapMode');

        for (var i = 0; i < mapModeBtns.length; i++) {
            addListener(mapModeBtns[i], 'click', function () {
                setMapMode(this.value);

                document.getElementById('mapModePanel').style.display = 'none';
            });
        }
    }

    function getMap() {
        var mapOptions = {
            credentials: "YOUR_BING_MAPS_KEY",
            showDashboard: false,
            showCopyright: false,
            showScalebar: false,
            enableSearchLogo: false,
            enableClickableLogo: false,
            backgroundColor: new Microsoft.Maps.Color(255, 0, 0, 0)
        };

        // Initialize the map
        map = new Microsoft.Maps.Map(document.getElementById("myMap"), mapOptions);

        gpsLayer = new Microsoft.Maps.EntityCollection();
        map.entities.push(gpsLayer);
    }

    function updateNavBar() {
        if (map.isRotationEnabled()) {
            document.getElementById('rotationBtns').style.display = '';
        } else {
            document.getElementById('rotationBtns').style.display = 'none';
        }
    }

    function search(query) {
        if (searchManager) {
            var request = {
                where: query,
                count: 1,
                callback: geocodeCallback,
                errorCallback: geocodeError
            };

            searchManager.geocode(request);
        } else {
            //Load the Search module and create a search manager.
            Microsoft.Maps.loadModule('Microsoft.Maps.Search', {
                callback: function () {
                    //Create the search manager
                    searchManager = new Microsoft.Maps.Search.SearchManager(map);

                    //Perfrom search logic
                    search(query);
                }
            });
        }
    }

    function geocodeCallback(response, userData) {
        if (response &&
            response.results &&
            response.results.length > 0) {
            var r = response.results[0];
            var l = new Microsoft.Maps.Location(r.location.latitude, r.location.longitude);

            //Display result on map        
            var p = new Microsoft.Maps.Pushpin(l);
            map.entities.push(p);

            //Zoom to result
            map.setView({ center: l, zoom: 15 });
        } else {
            showMessage("Not results found.");
        }

        document.getElementById('searchPanel').style.display = 'none';
    }

    function geocodeError(request) {
        showMessage("Unable to Geocode request.");

        document.getElementById('searchPanel').style.display = 'none';
    }

    function toggleGPS() {
        gpsEnabled = !gpsEnabled;

        // Initialize the location provider
        if (!geoLocationProvider) {
            geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);
        }

        //Clear the GPS layer 
        gpsLayer.clear();

        if (gpsEnabled) {
            // Get the user's current location
            geoLocationProvider.getCurrentPosition({
                successCallback: function (e) {
                    gpsLayer.push(new Microsoft.Maps.Pushpin(e.center));
                },
                errorCallback: function (e) {
                    showMessage(e.internalError);
                }
            });
        } else {
            //Remove the accuracy circle and cancel any request that might be processing
            geoLocationProvider.removeAccuracyCircle();
            geoLocationProvider.cancelCurrentRequest();
        }
    }

    function toggleTraffic() {
        trafficEnabled = !trafficEnabled;

        //Check to see if the traffic layer exists
        if (trafficLayer) {
            if (trafficEnabled) {
                trafficLayer.show();
            } else {
                trafficLayer.hide();
            }
        } else {
            //Load the traffic module and create the traffic layer.
            Microsoft.Maps.loadModule('Microsoft.Maps.Traffic', {
                callback: function () {
                    //Create the traffic layer
                    trafficLayer = new Microsoft.Maps.Traffic.TrafficLayer(map);

                    //Get the base tile layer and set the opacity
                    var layer = trafficLayer.getTileLayer();
                    layer.setOptions({ opacity: 0.5 });

                    trafficLayer.show();
                }
            });
        }
    }

    function setMapMode(mode) {
        var m;

        switch (mode) {
            case 'auto':
                m = Microsoft.Maps.MapTypeId.auto;
                break;
            case 'aerial':
                m = Microsoft.Maps.MapTypeId.aerial;
                break;
            case 'birdseye':
                m = Microsoft.Maps.MapTypeId.birdseye;
                break;
            case 'os':
                m = Microsoft.Maps.MapTypeId.ordnanceSurvey;
                break;
            case 'road':
            default:
                m = Microsoft.Maps.MapTypeId.road;
                break;
        }

        map.setView({ mapTypeId: m });
    }

    function showMessage(msg) {
        try{
            alert(msg);
        }
        catch(e){        
            if (Windows != null &&
                Windows.UI != null &&
                Windows.UI.Popups != null) {
                var popup = Windows.UI.Popups.MessageDialog(msg);
                popup.showAsync();
            }
        }
    }

    //Cross browser support for adding events. Mainly for IE7/8
    function addListener(element, eventName, eventHandler) {
        if (element.addEventListener) {
            element.addEventListener(eventName, eventHandler, false);
        } else if (element.attachEvent) {
            if (eventName == 'DOMContentLoaded') {
                eventName = 'readystatechange';
            }
            element.attachEvent('on' + eventName, eventHandler);
        }
    }

    addListener(document, 'DOMContentLoaded', init);
})();
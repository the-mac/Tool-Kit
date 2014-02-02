/// <reference path="../GeneratedArtifacts/viewModel.js" />

// Code examples for LightSwitch HTML client apps. To learn how to use this code, see http://msdn.microsoft.com/en-us/library/jj733572.aspx.

// Set a default value on a data entry screen
myapp.EntityName.created = function (entity) {
    entity.OrderDate = new Date();
    entity.OrderStatus = 'New';
};

// Format a number
myapp.MyScreen.MyNumericField_postRender = function (element, contentItem) {
    contentItem.dataBind("value", function (value) {
        if (value) {
            $(element).text(value.toFixed(2));
        }
    });
};

// Validate data on a screen
myapp.MyScreen.beforeApplyChanges = function (screen) {
    if (screen.Contact.ContactName.indexOf('!') != -1) {

        screen.findContentItem("ContactName").validationResults = [

        new msls.ValidationResult(

        screen.Contact.details.properties.contactName,

        "Contact Name cannot contain the character '!'.")

        ];

        return false;

    }
};

// Delete the selected item on a screen or popup
myapp.MyScreen.MyButton_execute = function (screen) {
    screen.getCustomers().then(function (customers) {
        customers.deleteSelected();
    });
};

// Create a custom modal picker by using a popup
myapp.MyScreen.created = function (screen) {
    screen.findContentItem("Products").dataBind("value.selectedItem", function (newValue) {
        if (newValue !== undefined && newValue !== null) {
            //Whenever selectedItem for Products changes, update the Product value on the main page 
            screen.Order_Detail.setProduct(screen.Products.selectedItem);

            //Close popup, if one is up. 
            screen.closePopup();
        }
    });
};

// Show a message box, and respond to a user selection
myapp.MyScreen.MyMethod_execute = function (screen) {
    msls.showMessageBox("Please choose the appropriate button", {

        title: "This is a message box",

        buttons: msls.MessageBoxButtons.yesNoCancel

    }).then(function (result) {

        if (result === msls.MessageBoxResult.yes) {

            alert("Yes button was chosen");

        }

        else if (result === msls.MessageBoxResult.no) {

            alert("No button was chosen");

        }

        else if (result === msls.MessageBoxResult.cancel) {

            alert("Please choose either Yes or No");

        }

    });
};

// Set the screen title dynamically
myapp.MyScreen.created = function (screen) {

    var name;

    name = screen.Customer.CompanyName;
    screen.details.displayName = "Information about: " + name;
};

// Enable or disable a button
myapp.MyScreen.MyMethod_execute = function (screen) {
    screen.findContentItem("MyButton").isEnabled = false;
};

myapp.MyScreen.MyMethod_execute = function (screen) {
    var result = false;
    if (!screen.Order.details.properties.Photo.isLoaded) {
        screen.Order.getPhoto();
    } else {
        screen.Order.getPhoto().then(function (ph) {
            result = !ph;
        });
    }
    return result;
};

// Customize the Save command to save to multiple data sources
myapp.onsavechanges = function (e) {

    var promises = [];

    promises.push(myapp.activeDataWorkspace.NorthwindData.saveChanges());

    promises.push(myapp.activeDataWorkspace.ApplicationData.saveChanges());

    e.detail.promise = WinJS.Promise.join(promises);

};

// Use a JQuery Mobile control
myapp.MyScreen.MyCustomControl_render = function (element, contentItem) {
    createSlider(element, contentItem, 0, 100);
};

function createSlider(element, contentItem, min, max) {
    // Generate the input element.
    $(element).append('<input type="range" min="' + min +
        '" max="' + max + '" value="' + contentItem.value + '" />');
};


// Customize a FlipSwitch control
myapp.MyScreen.MyFlipSwitch_render = function (element, contentItem) {
    createBooleanSwitch(element, contentItem);
};

function createBooleanSwitch(element, contentItem) {
    var $flipSwitch = $('<select data-role="slider"></select>').appendTo($(element));
    $('<option value="false">false</option>').appendTo($flipSwitch);
    $('<option value="true">true</option>').appendTo($flipSwitch);

    // set select value to match the original contentItem.value
    $flipSwitch.val((contentItem.value) ? "true" : "false");

    // add listener to update contentItem's value if slider changes
    $flipSwitch.change(function () {
        contentItem.value = ($flipSwitch.val() == "true");
    });

    // visually refresh the slider.
    $flipSwitch.slider().slider("refresh");
};

// Get the location of a device
myapp.MyScreen.MyMethod_execute = function (screen) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (pos) {
            screen.MyLocation.latitude = pos.coords.latitude.toString();
            screen.MyLocation.longitude = pos.coords.longitude.toString();
        });
    }
    else {
        alert("Geolocation not supported");
    }
};

// Display a location on a map

/// <reference path="jquery-1.7.1.js" />
/// <reference path="jquery.mobile-1.1.1.js" />
/// <reference path="msls-1.0.0.js" />

(function ($) {
    var _credentialsKey = "Ao75sYhQSfLgssT0QkO9n22xt0lgxzntrZ1xpCwLOC-kGhI584OYED3viFXLIWgC";

    // load the directions module only once per session
    Microsoft.Maps.loadModule('Microsoft.Maps.Directions');

    $.widget("msls.lightswitchBingMapsControl", {
        options: {
            mapType: Microsoft.Maps.MapTypeId.road,
            zoom: 3,
            showDashboard: false
        },

        _create: function () {
        },

        _init: function () {
            this.createMap();
        },

        destroy: function () {
            this._destroyBingMapsControl();
        },

        createMap: function () {
            this.htmlMapElement = this.element[0];

            // create empty map
            this.map = new Microsoft.Maps.Map(this.htmlMapElement,
                                {
                                    credentials: _credentialsKey,
                                    mapTypeId: this.options.mapType,
                                    zoom: this.options.zoom,
                                    showDashboard: this.options.showDashboard
                                });
        },

        addPinAsync: function (street, city, country, i, callback) {

            var widgetInstance = this;

            // construct a request to the REST geocode service using the widget's
            // optional parameters
            var geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations/" +
                                 street + "," + city + "," + country +
                                 "?key=" + _credentialsKey;

            // make the ajax request to the Bing Maps geocode REST service
            $.ajax({
                url: geocodeRequest,
                dataType: 'jsonp',
                async: true,
                jsonp: 'jsonp',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus + " " + errorThrown);
                },
                success: function (result) {
                    var coordinates = null;

                    if (result && result.resourceSets && (result.resourceSets.length > 0) &&
                        result.resourceSets[0].resources && (result.resourceSets[0].resources.length > 0)) {

                        // create a location based on the geocoded coordinates
                        coordinates = result.resourceSets[0].resources[0].point.coordinates;

                        widgetInstance._createPinFromCoordinates(coordinates, i, callback);
                    }
                }
            });
        },

        _createPinFromCoordinates: function (coordinates, i, callback) {
            var location = new Microsoft.Maps.Location(coordinates[0], coordinates[1]);
            var pin = new Microsoft.Maps.Pushpin(location, { text: '' + i + '' });
            Microsoft.Maps.Events.addHandler(pin, 'click', callback);
            this.map.entities.push(pin);
        },

        resetMap: function () {
            this.map.entities.clear();
        },

        _handleError: function (error) {
            alert("An error occurred.  " + error.message);
        },

        _destroyBingMapsControl: function () {
            if (this.map != null) {
                this.map.dispose();
                this.map = null;
            }
        }
    });
}(jQuery));

/// <reference path="../GeneratedArtifacts/viewModel.js" />

var mapDiv;
var current = 0;
var step = 15;

myapp.BrowseCustomers.Customer_render = function (element, contentItem) {
    mapDiv = $('<div />').appendTo($(element));
    $(mapDiv).lightswitchBingMapsControl();

    var visualCollection = contentItem.value;
    if (visualCollection.isLoaded) {
        showItems(current, step, contentItem.screen);
    } else {
        visualCollection.addChangeListener("isLoaded", function () {
            showItems(current, step, contentItem.screen);
        });
        visualCollection.load();
    }
};

function showItems(start, end, screen) {
    $(mapDiv).lightswitchBingMapsControl("resetMap");

    $.each(screen.Customers.data, function (i, customer) {
        if (i >= start && i <= end) {
            $(mapDiv).lightswitchBingMapsControl("addPinAsync", customer.Address,
                customer.City, customer.Country, i + 1, function () {
                    screen.Customers.selectedItem = customer;
                    screen.showPopup("Details");
                });
        }
    });
};
// Add the following to the default.htm file:
// <script type="text/javascript" charset="utf­8" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0"></script>
// <script type="text/javascript" charset="utf­8" src="Scripts/lightswitch.bing-maps.js"></script>

// Show a numeric keyboard on a device
myapp.MyScreen.NumericField_postRender = function (element, contentItem) {
    $(element).find("input").get(0).type = "number";
};

// Adjust the UI for different form factors
myapp.MyScreen.FieldToHide_postRender = function (element, contentItem) {
    $(element).addClass("hidden-on-phone");
};
// Add the following to the @media section of the user-customizations.css file.
// .hidden-on-phone {
//     display: none;
// }

// Render HTML data directly on a screen
myapp.MyScreen.MyCustomControl_render = function (element, contentItem) {
    element.innerHTML = contentItem.stringValue;
};

// Display a title on a popup
myapp.MyScreen.LocalProperty_postRender = function (element, contentItem) {
    element.textContent = "This is the title";
    $(element).css("font-size", "23px");
    $(element).css("font-weight", "bold");
};
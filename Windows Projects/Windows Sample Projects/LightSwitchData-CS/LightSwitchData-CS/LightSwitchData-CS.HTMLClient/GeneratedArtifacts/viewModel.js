/// <reference path="data.js" />

(function (lightSwitchApplication) {

    var $Screen = msls.Screen,
        $defineScreen = msls._defineScreen,
        $DataServiceQuery = msls.DataServiceQuery,
        $toODataString = msls._toODataString,
        $defineShowScreen = msls._defineShowScreen;

    function BrowseCustomers(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseCustomers screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Customers" type="msls.VisualCollection" elementType="msls.application.Customer">
        /// Gets the customers for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseCustomers.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseCustomers", parameters);
    }

    function AddEditCustomer(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditCustomer screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Customer" type="msls.application.Customer">
        /// Gets or sets the customer for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditCustomer.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditCustomer", parameters);
    }

    function BrowseZipCodesSet(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseZipCodesSet screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="ZipCodesSet" type="msls.VisualCollection" elementType="msls.application.ZipCodes">
        /// Gets the zipCodesSet for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseZipCodesSet.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseZipCodesSet", parameters);
    }

    msls._addToNamespace("msls.application", {

        BrowseCustomers: $defineScreen(BrowseCustomers, [
            {
                name: "Customers", kind: "collection", elementType: lightSwitchApplication.Customer,
                createQuery: function () {
                    return this.dataWorkspace.ApplicationData.Customers;
                }
            }
        ], [
        ]),

        AddEditCustomer: $defineScreen(AddEditCustomer, [
            { name: "Customer", kind: "local", type: lightSwitchApplication.Customer }
        ], [
        ]),

        BrowseZipCodesSet: $defineScreen(BrowseZipCodesSet, [
            {
                name: "ZipCodesSet", kind: "collection", elementType: lightSwitchApplication.ZipCodes,
                createQuery: function () {
                    return this.dataWorkspace.ApplicationData.ZipCodesSet;
                }
            }
        ], [
        ]),

        showBrowseCustomers: $defineShowScreen(function showBrowseCustomers(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseCustomers screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseCustomers", parameters, options);
        }),

        showAddEditCustomer: $defineShowScreen(function showAddEditCustomer(Customer, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditCustomer screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditCustomer", parameters, options);
        }),

        showBrowseZipCodesSet: $defineShowScreen(function showBrowseZipCodesSet(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseZipCodesSet screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseZipCodesSet", parameters, options);
        })

    });

}(msls.application));

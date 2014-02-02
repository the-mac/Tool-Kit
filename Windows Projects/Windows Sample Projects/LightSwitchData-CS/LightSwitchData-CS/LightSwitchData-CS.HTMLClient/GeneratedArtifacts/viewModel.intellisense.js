/// <reference path="viewModel.js" />

(function (lightSwitchApplication) {

    var $element = document.createElement("div");

    lightSwitchApplication.BrowseCustomers.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseCustomers
        },
        CustomerList: {
            _$class: msls.ContentItem,
            _$name: "CustomerList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.BrowseCustomers,
            value: lightSwitchApplication.BrowseCustomers
        },
        Customer: {
            _$class: msls.ContentItem,
            _$name: "Customer",
            _$parentName: "CustomerList",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.BrowseCustomers,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseCustomers,
                _$entry: {
                    elementType: lightSwitchApplication.Customer
                }
            }
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "Customer",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.Customer,
            value: lightSwitchApplication.Customer
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "RowTemplate",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.Customer,
            value: String
        },
        Address: {
            _$class: msls.ContentItem,
            _$name: "Address",
            _$parentName: "RowTemplate",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.Customer,
            value: String
        },
        City: {
            _$class: msls.ContentItem,
            _$name: "City",
            _$parentName: "RowTemplate",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.Customer,
            value: String
        },
        State: {
            _$class: msls.ContentItem,
            _$name: "State",
            _$parentName: "RowTemplate",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.Customer,
            value: String
        },
        ZipCode: {
            _$class: msls.ContentItem,
            _$name: "ZipCode",
            _$parentName: "RowTemplate",
            screen: lightSwitchApplication.BrowseCustomers,
            data: lightSwitchApplication.Customer,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseCustomers
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseCustomers, {
        /// <field>
        /// Called when a new BrowseCustomers screen is created.
        /// <br/>created(msls.application.BrowseCustomers screen)
        /// </field>
        created: [lightSwitchApplication.BrowseCustomers],
        /// <field>
        /// Called before changes on an active BrowseCustomers screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseCustomers screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseCustomers],
        /// <field>
        /// Called after the CustomerList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CustomerList_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("CustomerList"); }],
        /// <field>
        /// Called after the Customer content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Customer_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("Customer"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("RowTemplate"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("Name"); }],
        /// <field>
        /// Called after the Address content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Address_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("Address"); }],
        /// <field>
        /// Called after the City content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        City_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("City"); }],
        /// <field>
        /// Called after the State content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        State_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("State"); }],
        /// <field>
        /// Called after the ZipCode content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ZipCode_postRender: [$element, function () { return new lightSwitchApplication.BrowseCustomers().findContentItem("ZipCode"); }]
    });

    lightSwitchApplication.AddEditCustomer.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditCustomer
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditCustomer,
            data: lightSwitchApplication.AddEditCustomer,
            value: lightSwitchApplication.AddEditCustomer
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditCustomer,
            data: lightSwitchApplication.AddEditCustomer,
            value: lightSwitchApplication.Customer
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditCustomer,
            data: lightSwitchApplication.Customer,
            value: lightSwitchApplication.Customer
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCustomer,
            data: lightSwitchApplication.Customer,
            value: String
        },
        Address: {
            _$class: msls.ContentItem,
            _$name: "Address",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCustomer,
            data: lightSwitchApplication.Customer,
            value: String
        },
        ZipCode: {
            _$class: msls.ContentItem,
            _$name: "ZipCode",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCustomer,
            data: lightSwitchApplication.Customer,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditCustomer
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditCustomer, {
        /// <field>
        /// Called when a new AddEditCustomer screen is created.
        /// <br/>created(msls.application.AddEditCustomer screen)
        /// </field>
        created: [lightSwitchApplication.AddEditCustomer],
        /// <field>
        /// Called before changes on an active AddEditCustomer screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditCustomer screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditCustomer],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditCustomer().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditCustomer().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditCustomer().findContentItem("left"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditCustomer().findContentItem("Name"); }],
        /// <field>
        /// Called after the Address content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Address_postRender: [$element, function () { return new lightSwitchApplication.AddEditCustomer().findContentItem("Address"); }],
        /// <field>
        /// Called after the ZipCode content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ZipCode_postRender: [$element, function () { return new lightSwitchApplication.AddEditCustomer().findContentItem("ZipCode"); }]
    });

    lightSwitchApplication.BrowseZipCodesSet.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseZipCodesSet
        },
        ZipCodesList: {
            _$class: msls.ContentItem,
            _$name: "ZipCodesList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseZipCodesSet,
            data: lightSwitchApplication.BrowseZipCodesSet,
            value: lightSwitchApplication.BrowseZipCodesSet
        },
        ZipCodes: {
            _$class: msls.ContentItem,
            _$name: "ZipCodes",
            _$parentName: "ZipCodesList",
            screen: lightSwitchApplication.BrowseZipCodesSet,
            data: lightSwitchApplication.BrowseZipCodesSet,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseZipCodesSet,
                _$entry: {
                    elementType: lightSwitchApplication.ZipCodes
                }
            }
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "ZipCodes",
            screen: lightSwitchApplication.BrowseZipCodesSet,
            data: lightSwitchApplication.ZipCodes,
            value: lightSwitchApplication.ZipCodes
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseZipCodesSet
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseZipCodesSet, {
        /// <field>
        /// Called when a new BrowseZipCodesSet screen is created.
        /// <br/>created(msls.application.BrowseZipCodesSet screen)
        /// </field>
        created: [lightSwitchApplication.BrowseZipCodesSet],
        /// <field>
        /// Called before changes on an active BrowseZipCodesSet screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseZipCodesSet screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseZipCodesSet],
        /// <field>
        /// Called after the ZipCodesList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ZipCodesList_postRender: [$element, function () { return new lightSwitchApplication.BrowseZipCodesSet().findContentItem("ZipCodesList"); }],
        /// <field>
        /// Called after the ZipCodes content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ZipCodes_postRender: [$element, function () { return new lightSwitchApplication.BrowseZipCodesSet().findContentItem("ZipCodes"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.BrowseZipCodesSet().findContentItem("RowTemplate"); }]
    });

}(msls.application));
/// <reference path="../Scripts/msls.js" />

window.myapp = msls.application;

(function (lightSwitchApplication) {

    var $Entity = msls.Entity,
        $DataService = msls.DataService,
        $DataWorkspace = msls.DataWorkspace,
        $defineEntity = msls._defineEntity,
        $defineDataService = msls._defineDataService,
        $defineDataWorkspace = msls._defineDataWorkspace,
        $DataServiceQuery = msls.DataServiceQuery,
        $toODataString = msls._toODataString;

    function Customer(entitySet) {
        /// <summary>
        /// Represents the Customer entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this customer.
        /// </param>
        /// <field name="Id" type="Number">
        /// Gets or sets the id for this customer.
        /// </field>
        /// <field name="RowVersion" type="Array">
        /// Gets or sets the rowVersion for this customer.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this customer.
        /// </field>
        /// <field name="Address" type="String">
        /// Gets or sets the address for this customer.
        /// </field>
        /// <field name="City" type="String">
        /// Gets or sets the city for this customer.
        /// </field>
        /// <field name="State" type="String">
        /// Gets or sets the state for this customer.
        /// </field>
        /// <field name="ZipCode" type="String">
        /// Gets or sets the zipCode for this customer.
        /// </field>
        /// <field name="details" type="msls.application.Customer.Details">
        /// Gets the details for this customer.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function ZipCodes(entitySet) {
        /// <summary>
        /// Represents the ZipCodes entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this zipCodes.
        /// </param>
        /// <field name="Id" type="Number">
        /// Gets or sets the id for this zipCodes.
        /// </field>
        /// <field name="RowVersion" type="Array">
        /// Gets or sets the rowVersion for this zipCodes.
        /// </field>
        /// <field name="ZipCode" type="String">
        /// Gets or sets the zipCode for this zipCodes.
        /// </field>
        /// <field name="Latitude" type="String">
        /// Gets or sets the latitude for this zipCodes.
        /// </field>
        /// <field name="Longitude" type="String">
        /// Gets or sets the longitude for this zipCodes.
        /// </field>
        /// <field name="City" type="String">
        /// Gets or sets the city for this zipCodes.
        /// </field>
        /// <field name="State" type="String">
        /// Gets or sets the state for this zipCodes.
        /// </field>
        /// <field name="County" type="String">
        /// Gets or sets the county for this zipCodes.
        /// </field>
        /// <field name="Type" type="String">
        /// Gets or sets the type for this zipCodes.
        /// </field>
        /// <field name="details" type="msls.application.ZipCodes.Details">
        /// Gets the details for this zipCodes.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function ApplicationData(dataWorkspace) {
        /// <summary>
        /// Represents the ApplicationData data service.
        /// </summary>
        /// <param name="dataWorkspace" type="msls.DataWorkspace">
        /// The data workspace that created this data service.
        /// </param>
        /// <field name="Customers" type="msls.EntitySet">
        /// Gets the Customers entity set.
        /// </field>
        /// <field name="ZipCodesSet" type="msls.EntitySet">
        /// Gets the ZipCodesSet entity set.
        /// </field>
        /// <field name="details" type="msls.application.ApplicationData.Details">
        /// Gets the details for this data service.
        /// </field>
        $DataService.call(this, dataWorkspace);
    };
    function DataWorkspace() {
        /// <summary>
        /// Represents the data workspace.
        /// </summary>
        /// <field name="ApplicationData" type="msls.application.ApplicationData">
        /// Gets the ApplicationData data service.
        /// </field>
        /// <field name="details" type="msls.application.DataWorkspace.Details">
        /// Gets the details for this data workspace.
        /// </field>
        $DataWorkspace.call(this);
    };

    msls._addToNamespace("msls.application", {

        Customer: $defineEntity(Customer, [
            { name: "Id", type: Number },
            { name: "RowVersion", type: Array },
            { name: "Name", type: String },
            { name: "Address", type: String },
            { name: "City", type: String },
            { name: "State", type: String },
            { name: "ZipCode", type: String }
        ]),

        ZipCodes: $defineEntity(ZipCodes, [
            { name: "Id", type: Number },
            { name: "RowVersion", type: Array },
            { name: "ZipCode", type: String },
            { name: "Latitude", type: String },
            { name: "Longitude", type: String },
            { name: "City", type: String },
            { name: "State", type: String },
            { name: "County", type: String },
            { name: "Type", type: String }
        ]),

        ApplicationData: $defineDataService(ApplicationData, lightSwitchApplication.rootUri + "/ApplicationData.svc", [
            { name: "Customers", elementType: Customer },
            { name: "ZipCodesSet", elementType: ZipCodes }
        ], [
            {
                name: "Customers_SingleOrDefault", value: function (Id) {
                    return new $DataServiceQuery({ _entitySet: this.Customers },
                        lightSwitchApplication.rootUri + "/ApplicationData.svc" + "/Customers(" + "Id=" + $toODataString(Id, "Int32?") + ")"
                    );
                }
            },
            {
                name: "ZipCodesSet_SingleOrDefault", value: function (Id) {
                    return new $DataServiceQuery({ _entitySet: this.ZipCodesSet },
                        lightSwitchApplication.rootUri + "/ApplicationData.svc" + "/ZipCodesSet(" + "Id=" + $toODataString(Id, "Int32?") + ")"
                    );
                }
            }
        ]),

        DataWorkspace: $defineDataWorkspace(DataWorkspace, [
            { name: "ApplicationData", type: ApplicationData }
        ])

    });

}(msls.application));

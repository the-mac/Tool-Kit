/// <reference path="data.js" />

(function (lightSwitchApplication) {

    msls._addEntryPoints(lightSwitchApplication.Customer, {
        /// <field>
        /// Called when a new customer is created.
        /// <br/>created(msls.application.Customer entity)
        /// </field>
        created: [lightSwitchApplication.Customer]
    });

    msls._addEntryPoints(lightSwitchApplication.ZipCodes, {
        /// <field>
        /// Called when a new zipCodes is created.
        /// <br/>created(msls.application.ZipCodes entity)
        /// </field>
        created: [lightSwitchApplication.ZipCodes]
    });

}(msls.application));

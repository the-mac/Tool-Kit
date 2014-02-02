using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security.Server;
namespace LightSwitchApplication
{
    public partial class ApplicationDataService
    {
        partial void Customers_Inserting(Customer entity)
        {
            ZipCodes zip = ZipCodesSet.Where(e => e.ZipCode == entity.ZipCode).Single();
            entity.City = zip.City;
            entity.State = zip.State;
        }
        partial void Customers_Updating(Customer entity)
        {
            ZipCodes zip = ZipCodesSet.Where(e => e.ZipCode == entity.ZipCode).Single();
            entity.City = zip.City;
            entity.State = zip.State;
        }
    }
}

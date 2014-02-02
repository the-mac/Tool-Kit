using System;

#if WIN_RT
using Windows.UI.Xaml.Data;
#elif WP8
using System.Windows.Data;
#endif

namespace BingMapsAR.Converters
{
    /// <summary>
    /// Entity Type documentation: http://msdn.microsoft.com/en-us/library/hh478191.aspx
    /// </summary>
    public class EntityTypeNameConverter : IValueConverter
    {
        #if WIN_RT
        public object Convert(object value, Type targetType, object parameter, string language)
        #elif WP8
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        #endif
        {
            if (value is string)
            {
                switch (value as string)
                {
                    case "2084":
                     return "Winery";
                    case "3578":
                     return "ATM";
                    case "4013":
                     return "Train Station";
                    case "4100":
                     return "Commuter Rail Station";
                    case "4170":
                     return "Bus Station";
                    case "4444":
                     return "Named Place";
                    case "4482":
                     return "Ferry Terminal";
                    case "4493":
                     return "Marina";
                    case "4580":
                     return "Public Sports Airport";
                    case "4581":
                     return "Airport";
                    case "5000":
                     return "Business Facility";
                    case "5400":
                     return "Grocery Store";
                    case "5511":
                     return "Auto Dealerships";
                    case "5512":
                     return "Auto Dealership-Used Cars";
                    case "5540":
                     return "Petrol/Gasoline Station";
                    case "5571":
                     return "Motorcycle Dealership";
                    case "5800":
                     return "Restaurant";
                    case "5813":
                     return "Nightlife";
                    case "5999":
                     return "Historical Monument";
                    case "6000":
                     return "Bank";
                    case "6512":
                     return "Shopping";
                    case "7011":
                     return "Hotel";
                    case "7012":
                     return "Ski Resort";
                    case "7013":
                     return "Other Accommodation";
                    case "7014":
                     return "Ski Lift";
                    case "7389":
                     return "Tourist Information";
                    case "7510":
                     return "Rental Car Agency";
                    case "7520":
                     return "Parking Lot";
                    case "7521":
                     return "Parking Garage/House";
                    case "7522":
                     return "Park & Ride";
                    case "7538":
                     return "Auto Service & Maintenance";
                    case "7832":
                     return "Cinema";
                    case "7897":
                     return "Rest Area";
                    case "7929":
                     return "Performing Arts";
                    case "7933":
                     return "Bowling Centre";
                    case "7940":
                     return "Sports Complex";
                    case "7947":
                     return "Park/Recreation Area";
                    case "7985":
                     return "Casino";
                    case "7990":
                     return "Convention/Exhibition Centre";
                    case "7992":
                     return "Golf Course";
                    case "7994":
                     return "Civic/Community Centre";
                    case "7996":
                     return "Amusement Park";
                    case "7997":
                     return "Sports Centre";
                    case "7998":
                     return "Ice Skating Rink";
                    case "7999":
                     return "Tourist Attraction";
                    case "8060":
                     return "Hospital";
                    case "8200":
                     return "Higher Education";
                    case "8211":
                     return "School";
                    case "8231":
                     return "Library";
                    case "8410":
                     return "Museum";
                    case "8699":
                     return "Automobile Club";
                    case "9121":
                     return "City Hall";
                    case "9211":
                     return "Court House";
                    case "9221":
                     return "Police Station";
                    case "9500":
                     return "Business Service";
                    case "9501":
                     return "Other Communication";
                    case "9502":
                     return "Telephone Service";
                    case "9503":
                     return "Cleaning & Laundry";
                    case "9504":
                     return "Hair & Beauty";
                    case "9505":
                     return "Health Care Service";
                    case "9506":
                     return "Mover";
                    case "9507":
                     return "Photography";
                    case "9508":
                     return "Video & Game Rental";
                    case "9509":
                     return "Storage";
                    case "9510":
                     return "Tailor & Alteration";
                    case "9511":
                     return "Tax Service";
                    case "9512":
                     return "Repair Service";
                    case "9513":
                     return "Retirement/Nursing Home";
                    case "9514":
                     return "Social Service";
                    case "9515":
                     return "Utilities";
                    case "9516":
                     return "Waste & Sanitary";
                    case "9517":
                     return "Campground";
                    case "9518":
                     return "Auto Parts";
                    case "9519":
                     return "Car Wash/Detailing";
                    case "9520":
                     return "Local Transit";
                    case "9521":
                     return "Travel Agent & Ticketing";
                    case "9522":
                     return "Truck Stop/Plaza";
                    case "9523":
                     return "Church";
                    case "9524":
                     return "Synagogue";
                    case "9525":
                     return "Government Office";
                    case "9527":
                     return "Fire Department";
                    case "9528":
                     return "Road Assistance";
                    case "9529":
                     return "Funeral Director";
                    case "9530":
                     return "Post Office";
                    case "9531":
                     return "Banquet Hall";
                    case "9532":
                     return "Bar or Pub";
                    case "9533":
                     return "Cocktail Lounge";
                    case "9534":
                     return "Night Club";
                    case "9535":
                     return "Convenience Store";
                    case "9536":
                     return "Specialty Food Store";
                    case "9537":
                     return "Clothing Store";
                    case "9538":
                     return "Men's Apparel";
                    case "9539":
                     return "Shoe Store";
                    case "9540":
                     return "Specialty Clothing Store";
                    case "9541":
                     return "Women's Apparel";
                    case "9542":
                     return "Check Cashing Service";
                    case "9543":
                     return "Currency Exchange";
                    case "9544":
                     return "Money Transferring Service";
                    case "9545":
                     return "Department Store";
                    case "9546":
                     return "Discount Store";
                    case "9547":
                     return "Other General Merchandise";
                    case "9548":
                     return "Variety Store";
                    case "9549":
                     return "Garden Center";
                    case "9550":
                     return "Glass & Window";
                    case "9551":
                     return "Hardware Store";
                    case "9552":
                     return "Home Center";
                    case "9553":
                     return "Lumber";
                    case "9554":
                     return "Other House & Garden";
                    case "9555":
                     return "Paint";
                    case "9556":
                     return "Entertainment Electronics";
                    case "9557":
                     return "Floor & Carpet";
                    case "9558":
                     return "Furniture Store";
                    case "9559":
                     return "Major Appliance";
                    case "9560":
                     return "Home Specialty Store";
                    case "9561":
                     return "Computer & Software";
                    case "9562":
                     return "Flowers & Jewelry";
                    case "9563":
                     return "Gift, Antique, & Art";
                    case "9564":
                     return "Optical";
                    case "9565":
                     return "Pharmacy";
                    case "9566":
                     return "Record, CD, & Video";
                    case "9567":
                     return "Specialty Store";
                    case "9568":
                     return "Sporting Goods Store";
                    case "9569":
                     return "Wine & Liquor";
                    case "9570":
                     return "Boating";
                    case "9571":
                     return "Theater";
                    case "9572":
                     return "Race Track";
                    case "9573":
                     return "Golf Practice Range";
                    case "9574":
                     return "Health Club";
                    case "9575":
                     return "Bowling Alley";
                    case "9576":
                     return "Sports Activities";
                    case "9577":
                     return "Recreation Center";
                    case "9578":
                     return "Attorney";
                    case "9579":
                     return "Dentist";
                    case "9580":
                     return "Physician";
                    case "9581":
                     return "Realtor";
                    case "9582":
                     return "RV Park";
                    case "9583":
                     return "Medical Service";
                    case "9584":
                     return "Police Service";
                    case "9585":
                     return "Veterinarian Service";
                    case "9586":
                     return "Sporting & Instructional Camp";
                    case "9587":
                     return "Agricultural Product Market";
                    case "9589":
                     return "Public Restroom";
                    case "9590":
                     return "Residential Area/Building";
                    case "9591":
                     return "Cemetery";
                    case "9592":
                     return "Highway Exit";
                    case "9593":
                     return "Transportation Service";
                    case "9594":
                     return "Lottery Booth";
                    case "9707":
                     return "Public Transit Stop";
                    case "9708":
                     return "Public Transit Access";
                    case "9709":
                     return "Neighborhood";
                    case "9710":
                     return "Weigh Station";
                    case "9714":
                     return "Cargo Centre";
                    case "9715":
                     return "Military Base";
                    case "9717":
                     return "Tollbooth (China/Korea)";
                    case "9718":
                     return "Animal Park";
                    case "9719":
                     return "Truck Dealership";
                    case "9720":
                     return "Truck Parking";
                    case "9986":
                     return "Home Improvement & Hardware Store";
                    case "9987":
                     return "Consumer Electronics Store";
                    case "9988":
                     return "Office Supply & Services Store";
                    case "9989":
                     return "Taxi Stand";
                    case "9990":
                     return "Premium Default";
                    case "9991":
                     return "Industrial Zone";
                    case "9992":
                     return "Place of Worship";
                    case "9993":
                     return "Embassy";
                    case "9994":
                     return "County Council";
                    case "9995":
                     return "Bookstore";
                    case "9996":
                     return "Coffee Shop";
                    case "9998":
                     return "Hamlet";
                    case "9999":
                     return "Border Crossing";
                    default:
                     break;
                }
            }

            return "Unknown";
        }

        #if WIN_RT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        #elif WP8
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        #endif
        {
            throw new NotImplementedException();
        }
    }
}

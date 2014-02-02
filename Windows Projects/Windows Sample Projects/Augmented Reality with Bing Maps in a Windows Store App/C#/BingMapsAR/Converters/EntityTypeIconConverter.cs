using System;

#if WIN_RT
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
#elif WP8
using System.Windows.Data;
using System.Windows.Media.Imaging;
#endif

namespace BingMapsAR.Converters
{
    public class EntityTypeIconConverter : IValueConverter
    {
        #if WIN_RT
        public object Convert(object value, Type targetType, object parameter, string language)
        #elif WP8
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        #endif
        {
            if (value is string)
            {
                string iconName = string.Empty;

                switch (value as string)
                {
                    case "4581":    //Airport
                    case "4580":    //Public Sports Airport
                        iconName = "Airport";
                        break;
                    case "6000":    //Bank
                        iconName = "Bank";
                        break;
                    case "9532":	//Bar or Pub
                    case "2084":	//Winery
                        iconName = "Beer";
                        break;
                    case "9523":    //Church
                    case "9992":	//Place of Worship
                        iconName = "Church";
                        break;
                    case "9996":	//Coffee Shop
                        iconName = "Coffee";
                        break;
                    case "9527":	//Fire Department
                        iconName = "FireTruck";
                        break;
                    case "7538":    //Auto Service & Maintenance
                    case "9518":	//Auto Parts
                    case "9519":	//Car Wash/Detailing
                    case "5511":	//Auto Dealerships
                    case "5512":	//Auto Dealership-Used Cars
                    case "5571":	//Motorcycle Dealership
                    case "9512":	//Repair Service
                        iconName = "Garage";
                        break;
                    case "5540":    //Petrol/Gasoline Station
                        iconName = "GasStation";
                        break;
                    case "8060":    //Hospital
                    case "9583":    //Medical Service
                        iconName = "Hospital";
                        break;
                    case "7011":    //Hotel
                    case "7013":    //Other Accommodation
                        iconName = "Hotel";
                        break;
                    case "9565":    //Pharmacy
                        iconName = "Pharmacy";
                        break;
                    case "9221":	//Police Station
                    case "9584":	//Police Service
                        iconName = "PoliceCar";
                        break;
                    case "7947":	//Park/Recreation Area
                        iconName = "Tree";
                        break;
                    case "6512":    //Shopping"
                    case "9535":	//Convenience Store
                    case "9537":	//Clothing Store
                    case "9538":	//Men's Apparel
                    case "9539":	//Shoe Store
                    case "9540":	//Specialty Clothing Store
                    case "9541":	//Women's Apparel
                    case "9545":	//Department Store
                    case "9546":	//Discount Store
                    case "9547":	//Other General Merchandise
                    case "9548":	//Variety Store
                    case "9549":	//Garden Center
                    case "9550":	//Glass & Window
                    case "9551":	//Hardware Store
                    case "9552":	//Home Center
                    case "9553":	//Lumber
                    case "9554":	//Other House & Garden
                    case "9555":	//Paint
                    case "9556":	//Entertainment Electronics
                    case "9557":	//Floor & Carpet
                    case "9558":	//Furniture Store
                    case "9559":	//Major Appliance
                    case "9560":	//Home Specialty Store
                    case "9561":	//Computer & Software
                    case "9562":	//Flowers & Jewelry
                    case "9563":	//Gift, Antique, & Art
                    case "9564":	//Optical
                    case "9566":	//Record, CD, & Video
                    case "9567":	//Specialty Store
                    case "9568":	//Sporting Goods Store
                    case "9719":	//Truck Dealership
                    case "9986":	//Home Improvement & Hardware Store
                    case "9987":	//Consumer Electronics Store
                    case "9988":	//Office Supply & Services Store
                    case "9995":	//Bookstore
                        iconName = "ShoppingBags";
                        break;
                    case "5400":    //Grocery Store
                    case "9536":	//Specialty Food Store
                    case "9569":	//Wine & Liquor
                        iconName = "ShoppingCart";
                        break;
                    case "9572":	//Race Track
                    case "7992":	//Golf Course
                    case "9573":	//Golf Practice Range
                    case "9574":	//Health Club
                    case "9575":	//Bowling Alley
                    case "9576":	//Sports Activities
                    case "9577":	//Recreation Center
                    case "7933":	//Bowling Centre
                    case "7940":	//Sports Complex
                    case "7997":	//Sports Centre
                    case "7998":	//Ice Skating Rink
                        iconName = "Stadium";
                        break; 

                    
                    /*
                    case "3578":	//ATM
                    case "4013":	//Train Station
                    case "4100":	//Commuter Rail Station
                    case "4170":	//Bus Station
                    case "4444":	//Named Place
                    case "4482":	//Ferry Terminal
                    case "4493":	//Marina

                    case "5000":	//Business Facility

                    
                    case "5800":	//Restaurant
                    case "5813":	//Nightlife
                    case "5999":	//Historical Monument

                    case "7012":	//Ski Resort

                    case "7014":	//Ski Lift
                    case "7389":	//Tourist Information
                    case "7510":	//Rental Car Agency
                    case "7520":	//Parking Lot
                    case "7521":	//Parking Garage/House
                    case "7522":	//Park & Ride

                    case "7832":	//Cinema
                    case "7897":	//Rest Area
                    case "7929":	//Performing Arts
                    
                    
                    case "7985":	//Casino
                    case "7990":	//Convention/Exhibition Centre
                    
                    case "7994":	//Civic/Community Centre
                    case "7996":	//Amusement Park
                    
                    case "7999":	//Tourist Attraction

                    case "8200":	//Higher Education
                    case "8211":	//School
                    case "8231":	//Library
                    case "8410":	//Museum
                    case "8699":	//Automobile Club
                    case "9121":	//City Hall
                    case "9211":	//Court House
                    
                    case "9500":	//Business Service
                    case "9501":	//Other Communication
                    case "9502":	//Telephone Service
                    case "9503":	//Cleaning & Laundry
                    case "9504":	//Hair & Beauty
                    case "9505":	//Health Care Service
                    case "9506":	//Mover
                    case "9507":	//Photography
                    case "9508":	//Video & Game Rental
                    case "9509":	//Storage
                    case "9510":	//Tailor & Alteration
                    case "9511":	//Tax Service

                    case "9513":	//Retirement/Nursing Home
                    case "9514":	//Social Service
                    case "9515":	//Utilities
                    case "9516":	//Waste & Sanitary
                    case "9517":	//Campground
                    
                    case "9520":	//Local Transit
                    case "9521":	//Travel Agent & Ticketing
                    case "9522":	//Truck Stop/Plaza

                    case "9524":	//Synagogue
                    case "9525":	//Government Office
                    
                    case "9528":	//Road Assistance
                    case "9529":	//Funeral Director
                    case "9530":	//Post Office
                    case "9531":	//Banquet Hall
                    
                    case "9533":	//Cocktail Lounge
                    case "9534":	//Night Club
                    
                    case "9542":	//Check Cashing Service
                    case "9543":	//Currency Exchange
                    case "9544":	//Money Transferring Service
                    
                    case "9570":	//Boating
                    case "9571":	//Theater
                    
                    case "9578":	//Attorney
                    case "9579":	//Dentist
                    case "9580":	//Physician
                    case "9581":	//Realtor
                    case "9582":	//RV Park

                    
                    case "9585":	//Veterinarian Service
                    case "9586":	//Sporting & Instructional Camp
                    case "9587":	//Agricultural Product Market
                    case "9589":	//Public Restroom
                    case "9590":	//Residential Area/Building
                    case "9591":	//Cemetery
                    case "9592":	//Highway Exit
                    case "9593":	//Transportation Service
                    case "9594":	//Lottery Booth
                    case "9707":	//Public Transit Stop
                    case "9708":	//Public Transit Access
                    case "9709":	//Neighborhood
                    case "9710":	//Weigh Station
                    case "9714":	//Cargo Centre
                    case "9715":	//Military Base
                    case "9717":	//Tollbooth (China/Korea)
                    case "9718":	//Animal Park
                    case "9720":	//Truck Parking

                    

                    case "9989":	//Taxi Stand
                    case "9990":	//Premium Default
                    case "9991":	//Industrial Zone
                    
                    case "9993":	//Embassy
                    case "9994":	//County Council
                    
                    case "9998":	//Hamlet
                    case "9999":	//Border Crossing
                        */
                    default:
                        iconName = "Other";
                        break;
                }

                #if WP8
                return new BitmapImage(new Uri(string.Format("/Assets/Icons/{0}.png", iconName), UriKind.Relative));
                #elif WIN_RT
                return new BitmapImage(new Uri(string.Format("ms-appx:///Assets/Icons/{0}.png", iconName)));
                #endif
            }

            return null;
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

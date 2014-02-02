
namespace BingMapsAR
{
    public class Helpers
    {
        /// <summary>
        /// Simple method that cleans a phone number and returns it in a user friendly format
        /// </summary>
        /// <param name="phoneNumber">Phone number to be formatted</param>
        /// <returns>User friendly phone number</returns>
        public static string FormatPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("-", "");

            if (phoneNumber.Length > 6)
            {
                return string.Format("({0}) {1}-{2}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6));
            }

            return phoneNumber;
        }
    }
}

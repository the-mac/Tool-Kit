using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O365_ReadPostFeedsWP8
{
    /// <summary>
    /// Class to set Office 365 property SiteUrl.
    /// </summary>
    public static class Office365
    {
        private static string _office365SiteUrl = "";
        public static string Office365SiteUrl
        {
            get { return _office365SiteUrl; }
            set { _office365SiteUrl = value; }
        }
    }
}

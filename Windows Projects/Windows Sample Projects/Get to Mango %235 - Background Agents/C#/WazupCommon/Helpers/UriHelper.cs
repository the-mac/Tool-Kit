using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wazup.Services;

namespace Wazup.Helpers
{
    public static class UriHelper
    {
        public static Uri MakeTrendUri(Trend trend)
        {
            return MakeTrendUri(trend.name);
        }

        public static Uri MakeTrendUri(string trendName)
        {
            return new Uri(String.Format(@"/Views/TwitterPage.xaml?trend={0}", HttpUtility.UrlEncode(trendName)), UriKind.Relative);
        }

        public static string GetTrendNameFromUri(Uri uri)
        {
            // Assumes that the only query information is Uri related
            return HttpUtility.UrlDecode(uri.OriginalString.Substring(uri.OriginalString.IndexOf('=') + 1));
        }
    }
}

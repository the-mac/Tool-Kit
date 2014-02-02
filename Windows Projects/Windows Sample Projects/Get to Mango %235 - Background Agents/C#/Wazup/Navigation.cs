using System;
using Microsoft.Phone.Controls;

namespace Wazup
{
    public enum ApplicationPages
    {
        Digg,
        Trends,
        Twitter,
        Blog
    }

    public static class Navigation
    {
        /// <summary>
        /// Goes to page.
        /// </summary>
        /// <param name="applicationPage">The application page.</param>
        public static void GoToPage(this PhoneApplicationPage phoneApplicationPage, ApplicationPages applicationPage)
        {
            switch (applicationPage)
            {
                case ApplicationPages.Digg:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/DiggPage.xaml", UriKind.Relative));
                    break;

                case ApplicationPages.Trends:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/TrendsPage.xaml", UriKind.Relative));
                    break;

                case ApplicationPages.Twitter:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/TwitterPage.xaml", UriKind.Relative));
                    break;

                case ApplicationPages.Blog:
                    phoneApplicationPage.NavigationService.Navigate(new Uri("/Views/BlogPage.xaml", UriKind.Relative));
                    break;
            }
        }
    }
}

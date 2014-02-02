/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8RssFeedSort
* Copyright (c) Microsoft Corporation
*
* This demo shows how to sort RssFeeds.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Xml;
using System.IO;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;

namespace CSWP8RssFeedSort
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Enum for sort type.
        public enum SortTypeEnum
        {
            PublishDate, Title, Authors
        }

        // Enum for sort order.
        public enum SortEnum
        {
            asc, desc
        }

        string strURL = "http://windowsteamblog.com/windows_phone/b/windowsphone/rss.aspx"; // URL of RssFeeds.
        SortTypeEnum tempSortType = SortTypeEnum.PublishDate;                               // Default (temporary) Sort type.
        SortEnum tempSortEnum = SortEnum.asc;                                               // Default (temporary) sort order.

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Click handler that runs when the 'Load Feed' or 'Refresh Feed' button is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFeedButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DownloadFeed();          
        }

        /// <summary>
        /// Test sort by Authors by using local data.
        /// </summary>
        private void SortByAuthors()
        {
            tempSortType = SortTypeEnum.Authors;

            // Load the feed into a SyndicationFeed instance.
            XmlReader xmlReader = XmlReader.Create("Feeds.xml");
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            // Update UI.
            UpdateUI(feed);
        }

        /// <summary>
        /// Download feed from the specified URL.
        /// </summary>
        private void DownloadFeed()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(new System.Uri(strURL));
        }

        /// <summary>
        /// Event handler which runs after the feed is fully downloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // Showing the exact error message is useful for debugging. In a finalized application, 
                    // output a friendly and applicable string to the user instead. 
                    MessageBox.Show(e.Error.Message);
                });
            }
            else
            {
                // Save the feed into the State property in case the application is tombstoned. 
                this.State["feed"] = e.Result;

                UpdateFeedList(e.Result);
            }
        }

        /// <summary>
        /// This method determines whether the user has navigated to the application after the application was tombstoned.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // First, check whether the feed is already saved in the page state.
            if (this.State.ContainsKey("feed"))
            {
                // Get the feed again only if the application was tombstoned, which means the ListBox will be empty.
                // This is because the OnNavigatedTo method is also called when navigating between pages in your application.
                // You would want to rebind only if your application was tombstoned and page state has been lost. 
                if (feedListBox.Items.Count == 0)
                {
                    UpdateFeedList(State["feed"] as string);
                }
            }
        }

        /// <summary>
        /// Use the Dispatcher to update the UI. This keeps the UI thread free from heavy processing.
        /// </summary>
        /// <param name="feed"></param>
        private void UpdateUI(SyndicationFeed feed)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                feedListBox.ItemsSource = SortByType(feed.Items, tempSortType, tempSortEnum);

                loadFeedButton.Content = "Refresh Feed";
            });
        }

        /// <summary>
        /// This method sets up the feed and binds it to our ListBox.
        /// </summary>
        /// <param name="feedXML"></param> 
        private void UpdateFeedList(string feedXML)
        {
            // Load the feed into a SyndicationFeed instance.
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            UpdateUI(feed);
        }

        /// <summary>
        /// Sort the list according to the specified sort type and order.
        /// </summary>
        /// <param name="items">RssFeeds</param>
        /// <param name="sortType">sort type</param>
        /// <param name="sortEnum">sort order</param>
        /// <returns></returns>
        private IEnumerable<SyndicationItem> SortByType(IEnumerable<SyndicationItem> items, SortTypeEnum sortType, SortEnum sortEnum)
        {
            // RssFeed list.
            List<SyndicationItem> feedItems = new List<SyndicationItem>(items);

            // Perform the sort.
            switch (sortType)
            {
                case SortTypeEnum.PublishDate:
                    if (sortEnum.Equals(SortEnum.desc))
                    {
                        // newest first                           
                        feedItems.Sort((a, b) =>
                        {
                            return b.PublishDate.CompareTo(a.PublishDate);
                        });
                    }
                    else
                    {
                        // oldest first
                        feedItems.Sort((a, b) =>
                        {
                            return a.PublishDate.CompareTo(b.PublishDate);
                        });
                    }
                    break;
                case SortTypeEnum.Title:
                    if (sortEnum.Equals(SortEnum.desc))
                    {
                        feedItems.Sort((a, b) =>
                        {
                            return b.Title.Text.CompareTo(a.Title.Text);
                        });
                    }
                    else
                    {
                        feedItems.Sort((a, b) =>
                        {
                            return a.Title.Text.CompareTo(b.Title.Text);
                        });
                    }
                    break;
                case SortTypeEnum.Authors:
                    if (sortEnum.Equals(SortEnum.desc))
                    {
                        feedItems.Sort((a, b) =>
                        {
                            return MyCompareClass.CompareSyndicationPerson(b, a);
                        });
                    }
                    else
                    {
                        feedItems.Sort((a, b) =>
                        {
                            return MyCompareClass.CompareSyndicationPerson(a, b);
                        });
                    }
                    break;
                default:
                    break;
            }

            return feedItems;
        }

        /// <summary>
        /// Set the sort order.
        /// </summary>
        private void SetSort()
        {
            if (tempSortEnum == SortEnum.desc)
            {
                tempSortEnum = SortEnum.asc;
            }
            else
            {
                tempSortEnum = SortEnum.desc;
            }
        }

        /// <summary>
        /// Sort by title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSortByTitle_Click(object sender, RoutedEventArgs e)
        {
            tempSortType = SortTypeEnum.Title;
            DownloadFeed();
            SetSort();  
        }

        /// <summary>
        /// Sort by Authors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSortByAuthors_Click(object sender, RoutedEventArgs e)
        {
            SortByAuthors();         
            SetSort();          
        }

        /// <summary>
        /// Sort by PublishDate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSortByPublishDate_Click(object sender, RoutedEventArgs e)
        {
            tempSortType = SortTypeEnum.PublishDate;
            DownloadFeed();
            SetSort();          
        }
    }

    /// <summary>
    ///  Helper class for sorting by Authors.
    /// </summary>
    public class MyCompareClass
    {
        /// <summary>
        /// Compare the two Collections.
        /// </summary>
        /// <typeparam name="T">Base Class</typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Compare<T>(Collection<T> a, Collection<T> b) where T : IComparable<T>
        {
            if (a.Count == b.Count)
            {
                int aTotalValue = 0;
                int bTotalValue = 0;

                for (int i = 0; i < a.Count; i++)
                {
                    if (a[i].CompareTo(b[i]) > 0)
                    {
                        aTotalValue++;
                    }
                    else if (a[i].CompareTo(b[i]) < 0)
                    {
                        bTotalValue++;
                    }
                }
                return aTotalValue - bTotalValue;
            }
            return a.Count - b.Count;
        }

        /// <summary>
        /// Compare SyndicationPerson for sorting. Use custom class to sort.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CompareSyndicationPerson(SyndicationItem a, SyndicationItem b)
        {
            Collection<SyndicationPerson> authors1 = a.Authors; // a's Authors
            Collection<SyndicationPerson> authors2 = b.Authors; // b's Authors

            Collection<SyndicationPersonTemp> tempAuthors1 = new Collection<SyndicationPersonTemp>();
            Collection<SyndicationPersonTemp> tempAuthors2 = new Collection<SyndicationPersonTemp>();

            // Store SyndicationPerson's data to SyndicationPersonTemp. Then we will 
            // sort the list of SyndicationPersonTemp.
            foreach (SyndicationPerson item in authors1)
            {
                tempAuthors1.Add(new SyndicationPersonTemp(item));
            }
            foreach (SyndicationPerson item in authors2)
            {
                tempAuthors2.Add(new SyndicationPersonTemp(item));
            }

            // Perform the compare.
            int result = Compare<SyndicationPersonTemp>(tempAuthors1, tempAuthors2);
            return result;
        }
    }

    /// <summary>
    /// This class is used to store SyndicationPerson's data for sorting.
    /// </summary>
    public class SyndicationPersonTemp : SyndicationPerson, IComparable<SyndicationPersonTemp>
    {
        // Constructor. Initialization SyndicationPersonTemp by using SyndicationPerson's data .
        public SyndicationPersonTemp(SyndicationPerson sp)
            : base()
        {
            this.Email = sp.Email;
            this.Name = sp.Name;
            this.Uri = sp.Uri;
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SyndicationPersonTemp other)
        {
            // Alphabetic sort if Email is not equal. [A to Z]
            if (this.Email != other.Email)
            {
                return this.Email.CompareTo(other.Email);
            }
            else if (this.Name != other.Name)
            {
                return this.Name.CompareTo(other.Name);
            }
            else if (this.Uri != other.Uri)
            {
                return this.Uri.CompareTo(other.Uri);
            }

            // Default to Email sort. [High to low]
            return this.Email.CompareTo(other.Email);
        }
    }
}
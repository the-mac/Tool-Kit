using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using O365_ReadPostFeedsWP8.Resources;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Microfeed;
using Microsoft.SharePoint.Client.Social;

namespace O365_ReadPostFeedsWP8
{
    public partial class ReadFeeds : PhoneApplicationPage, IDisposable
    {
        ClientContext context;
        string owner;

        // Constructor
        public ReadFeeds()
        {
            InitializeComponent();
        }

        private void btnGetFeeds_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(txtUsername.Text))
            {
                try
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            lstFeeds.Items.Clear();
                        });

                    // Create the client context object.
                    context = new ClientContext(Office365.Office365SiteUrl);

                    // Create an instance of Authenticator object.
                    Authenticator at = new Authenticator();
                    // Set AuthenticationMode for SharePoint Online.
                    at.AuthenticationMode = ClientAuthenticationMode.MicrosoftOnline;
                    at.CookieCachingEnabled = true;

                    // Assign the instance of Authenticator object to the ClientContext.Credential property.
                    context.Credentials = at;

                    string targetUser = txtUsername.Text;

                    // Create an instance of SocialFeedManager.
                    SocialFeedManager feedManager = new SocialFeedManager(context);
                    context.Load(feedManager, f => f.Owner);
                    SocialFeedOptions feedOptions = new SocialFeedOptions();
                    feedOptions.MaxThreadCount = 10;

                    // Get feeds for the target user.
                    ClientResult<SocialFeed> targetUserFeed = feedManager.GetFeedFor(targetUser, feedOptions);
                    feedOptions.SortOrder = SocialFeedSortOrder.ByCreatedTime;

                    context.ExecuteQueryAsync(
                        (object obj, ClientRequestSucceededEventArgs args) =>
                        {
                            // Success Logic.
                            owner = feedManager.Owner.Name;
                          
                            // Iterate through the feeds of target user.
                            IterateThroughFeed(targetUserFeed.Value, SocialFeedType.Personal);
                        },

                        (object obj, ClientRequestFailedEventArgs args) =>
                        {
                            // Failure Logic.
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    MessageBox.Show("Error:   " + args.Message);
                                });

                        });

                }
                catch (Exception ex)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
                       MessageBox.Show("Error:   " + ex.Message);
                   });
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Enter the target username whose feeds you want to retrieve");
                });
            }
        }

        /// <summary>
        /// Iterates through the feeds retrieved for the target user.
        /// </summary>
        /// <param name="feed">Collection of Feeds retrieved.</param>
        /// <param name="feedType">Type of feed.</param>
        public void IterateThroughFeed(SocialFeed feed, SocialFeedType feedType)
        {
            try
            {

                SocialThread[] threads = feed.Threads;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    lstFeeds.Items.Clear();
                });

                foreach (SocialThread thread in threads)
                {
                    if (thread.ThreadType == SocialThreadType.Normal)
                    {
                        // Get the root post text value and add to the list.
                        SocialPost rootPost = thread.RootPost;
                        Feed objFeed = new Feed();
                        objFeed.FeedText = rootPost.Text;
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                lstFeeds.Items.Add(objFeed);
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
               {
                   MessageBox.Show("Error:   " + ex.Message);
               });
            }

        }

        private void btnGoToPostFeed_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new PostFeed();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources.
                context.Dispose();
            }
            // Free native resources.
        }

       
    }
}
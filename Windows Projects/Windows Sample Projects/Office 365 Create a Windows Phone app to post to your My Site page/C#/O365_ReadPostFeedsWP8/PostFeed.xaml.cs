using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Microfeed;
using Microsoft.SharePoint.Client.Social;

namespace O365_ReadPostFeedsWP8
{
    public partial class PostFeed : PhoneApplicationPage, IDisposable
    {
        ClientContext context;

        public PostFeed()
        {
            InitializeComponent();
        }

        private void btnPostFeed_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPost.Text))
            {
                try
                {
                    
                    // Create the client context object.
                    context = new ClientContext(Office365.Office365SiteUrl);


                    // Create an instance of Authenticator object.
                    Authenticator at = new Authenticator();
                    at.AuthenticationMode = ClientAuthenticationMode.MicrosoftOnline;
                    at.CookieCachingEnabled = true;

                    // Assign the instance of Authenticator object to the ClientContext.Credential property.
                    context.Credentials = at;

                    // Create the SocialPostCreationData instance.
                    SocialPostCreationData postCreationData = new SocialPostCreationData();
                    // Add the content for the root post.
                    postCreationData.ContentText = txtPost.Text;

                    // Create an instance of SocialFeedManager.
                    SocialFeedManager feedManager = new SocialFeedManager(context);

                    // Create the root post.
                    feedManager.CreatePost(null, postCreationData);

                    context.ExecuteQueryAsync(
                        (object obj, ClientRequestSucceededEventArgs args) =>
                        {
                            // Success Logic.
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    MessageBox.Show("Post published to Office 365 My Site successfully.");
                                });

                        },

                        (object obj, ClientRequestFailedEventArgs args) =>
                        {
                            // Failure logic.
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                               {
                                   MessageBox.Show(args.Message);
                               });
                        });

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:   " + ex.Message);
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Enter text for the post.");
                });
            }


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
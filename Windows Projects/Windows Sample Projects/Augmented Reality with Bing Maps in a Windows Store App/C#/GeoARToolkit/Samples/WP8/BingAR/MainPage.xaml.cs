#region License
/******************************************************************************
 * COPYRIGHT © MICROSOFT CORP. 
 * MICROSOFT LIMITED PERMISSIVE LICENSE (MS-LPL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 * 1. Definitions
 * The terms “reproduce,” “reproduction,” “derivative works,” and “distribution” have the same meaning here as under U.S. copyright law.
 * A “contribution” is the original software, or any additions or changes to the software.
 * A “contributor” is any person that distributes its contribution under this license.
 * “Licensed patents” are a contributor’s patent claims that read directly on its contribution.
 * 2. Grant of Rights
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 * 3. Conditions and Limitations
 * (A) No Trademark License- This license does not grant you rights to use any contributors’ name, logo, or trademarks.
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 * (E) The software is licensed “as-is.” You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 * (F) Platform Limitation- The licenses granted in sections 2(A) & 2(B) extend only to the software or derivative works that you create that run on a Microsoft Windows operating system product.
 ******************************************************************************/
#endregion // License


// Uncomment to test win X3D library
//#define X3D
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using GART;
using GART.Data;
#if X3D
using GART.X3D;
#else
using Microsoft.Xna.Framework;
#endif
using BingAR.Bing.Search;
using Microsoft.Phone.Controls.Maps;
using BingAR.Data;
using System.Collections.ObjectModel;
using GART.Controls;
using GART.BaseControls;
using Microsoft.Phone.Controls.Maps.Platform;

using Location = Microsoft.Phone.Controls.Maps.Platform.Location;

namespace BingAR
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly Vector2 DistanceBeforeRefresh = new Vector2(90, 90); // Refresh search when we move 100 yards (90 meters)
        private const int MaxResultsPerQuery = 10;

        #region Member Variables
        private CredentialsProvider bingCredentialProvider;
        private Location lastSearchLocation;
        #endregion // Member Variables

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Get the Bing credentials from our app resources
            bingCredentialProvider = new ApplicationIdCredentialsProvider((App.Current.Resources[App.BingCredentialsKey] as GART.Controls.MapCredentials).ApplicationId);

            #region For Testing
            // Don't move
            // ARDisplay.LocationEnabled = false;

            // Pretend we're here
            // ARDisplay.Location = new GeoCoordinate(30.07370, -95.43430);
            #endregion // For Testing
        }

        #region Internal Methods
        /// <summary>
        /// Begins an asynchronous Bing search.
        /// </summary>
        private void BeginSearch()
        {
            // Last search location is now this location
            lastSearchLocation = ARDisplay.Location;

            // Get the credentials from the credential provider 
            // These credentials are used by both the map and the SOAP services
            // The fetch happens asynchronously

            
            bingCredentialProvider.GetCredentials(credentials =>
                {                   
                    SearchRequest searchRequest = new SearchRequest();
                    searchRequest.Credentials = new Bing.Search.Credentials();

                    // Set the credentials using a valid Bing Maps key
                    searchRequest.Credentials.ApplicationId = credentials.ApplicationId;
                    //searchRequest.Credentials.Token = string.Empty;
                    searchRequest.UserProfile = new UserProfile();
                    searchRequest.UserProfile.DistanceUnit = DistanceUnit.Kilometer; // Everything in the AR Toolkit is measured in meters

                    StructuredSearchQuery ssQuery = new StructuredSearchQuery();
                    ssQuery.Keyword = "restaurant";
                    ssQuery.Location = ARDisplay.Location.ToWGS84String();
                    searchRequest.StructuredQuery = ssQuery;
                    searchRequest.SearchOptions = new SearchOptions();
                    searchRequest.SearchOptions.Radius = 0.2; // 100 meters. Should be configurable. Right now the WorldView only shows up to 100 meters anyway.
                    searchRequest.SearchOptions.Count = MaxResultsPerQuery;
                    // searchRequest.SearchOptions.Filters = new FilterExpression() { PropertyId = 3, CompareOperator = CompareOperator.GreaterThanOrEquals, FilterValue = 8.16 };

                    // Make the search request (also async)
                    SearchServiceClient searchService = new SearchServiceClient();
                    searchService.SearchCompleted += new EventHandler<SearchCompletedEventArgs>(searchService_SearchCompleted);

                    searchService.SearchAsync(searchRequest);
                });
        }

        private void GoFixed()
        {
            // Don't move
            ARDisplay.LocationEnabled = false;

            // Pretend we're here
            ARDisplay.Location = new Location() { Latitude = 30.07370, Longitude = -95.43430 };

            // Search again
            BeginSearch();
        }

        private void searchService_SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            // Only proceed if the search was a success
            if ((e.Error == null) && (e.Result.ResponseSummary.StatusCode == ResponseStatusCode.Success))
            {
                // Clear out existing items
                ARDisplay.ARItems.Clear();

                // Add new results in
                foreach (SearchResultBase result in e.Result.ResultSets[0].Results)
                {
                    // See if this search result is a business result
                    BusinessSearchResult businessResult = result as BusinessSearchResult;

                    // If it's a business result, convert it to a restaurant item and add it
                    if (businessResult != null)
                    {
                        // Create the new restaurant item
                        RestaurantItem ri = new RestaurantItem()
                        {
                            Cuisine = BingDataHelper.GetCuisineName(businessResult),
                            GeoLocation = BingDataHelper.GetLocation(businessResult),
                            Name = businessResult.Name,
                            Rating = businessResult.UserRating,
                        };
                        
                        // Add to the collection
                        ARDisplay.ARItems.Add(ri);
                    }
                }
            }
        }

        /// <summary>
        /// Switches between rottaing the Heading Indicator or rotating the Map to the current heading.
        /// </summary>
        private void SwitchHeadingMode()
        {
            if (HeadingIndicator.RotationSource == RotationSource.AttitudeHeading)
            {
                HeadingIndicator.RotationSource = RotationSource.North;
                OverheadMap.RotationSource = RotationSource.AttitudeHeading;
            }
            else
            {
                OverheadMap.RotationSource = RotationSource.North;
                HeadingIndicator.RotationSource = RotationSource.AttitudeHeading;
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        private void ARDisplay_LocationChanged(object sender, EventArgs e)
        {
            // TODO: For now only search once on startup but later search after the user has traveled some distance
            if (lastSearchLocation == null)
            {
                BeginSearch();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Stop AR services
            ARDisplay.StopServices();

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Start AR services
            ARDisplay.StartServices();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// To support any orientation, override this method and call
        /// ARDisplay.HandleOrientationChange() method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            ControlOrientation orientation = ControlOrientation.Default;

            switch (e.Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    orientation = ControlOrientation.Clockwise270Degrees;
                    break;
                case PageOrientation.LandscapeRight:
                    orientation = ControlOrientation.Clockwise90Degrees;
                    break;
            }

            ARDisplay.Orientation = orientation;
        }

        private void HeadingButton_Click(object sender, System.EventArgs e)
        {
            UIHelper.ToggleVisibility(HeadingIndicator);
        }

        private void MapButton_Click(object sender, System.EventArgs e)
        {
            UIHelper.ToggleVisibility(OverheadMap);
        }

        private void RotateButton_Click(object sender, System.EventArgs e)
        {
            SwitchHeadingMode();
        }

        private void ThreeDButton_Click(object sender, System.EventArgs e)
        {
            UIHelper.ToggleVisibility(WorldView);
        }

        private void GoFixedMenuItem_Click(object sender, System.EventArgs e)
        {
            GoFixed();
        }
        #endregion // Overrides / Event Handlers

    }
}

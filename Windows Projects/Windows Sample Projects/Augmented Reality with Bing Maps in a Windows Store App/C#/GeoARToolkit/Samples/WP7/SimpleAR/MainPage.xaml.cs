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

using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
#if X3D
using GART.X3D;
using Matrix = GART.X3D.Matrix;
#else
using Microsoft.Xna.Framework;
using Matrix = Microsoft.Xna.Framework.Matrix;
#endif
using Point = System.Windows.Point;
using GART;
using GART.Controls;
using GART.Data;
using System.Device.Location;
using System;
using Microsoft.Phone.Controls.Maps.Platform;
using GART.BaseControls;

namespace SimpleAR
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Member Variables
        private Random rand = new Random();
        #endregion // Member Variables

        #region Constructors
        public MainPage()
        {
            InitializeComponent();
        }
        #endregion // Constructors

        #region Internal Methods
        private void AddLabel(Location location, string label)
        {
            // We'll use the specified text for the content and we'll let 
            // the system automatically project the item into world space
            // for us based on the Geo location.
            ARItem item = new ARItem()
            {
                Content = label,
                GeoLocation = location,
            };

            ARDisplay.ARItems.Add(item);
        }

        private void AddNearbyLabels()
        {
            // Start with the current location
            var current = ARDisplay.Location;

            // We'll add three Labels
            for (int i = 0; i < 3; i++)
            {
                // Create a new location based on the users location plus
                // a random offset.
                Location offset = new Location()
                    {
                        Latitude = current.Latitude + ((double)rand.Next(-60, 60)) / 100000,
                        Longitude = current.Longitude + ((double)rand.Next(-60, 60)) / 100000,
                        Altitude = Double.NaN // NaN will keep it on the horizon
                    };

                AddLabel(offset, "Location " + i);
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
        private void AddLocationsMenu_Click(object sender, EventArgs e)
        {
            AddNearbyLabels();
        }

        private void ClearLocationsMenu_Click(object sender, EventArgs e)
        {
            ARDisplay.ARItems.Clear();
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
            ARDisplay.HandleOrientationChange(e);
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
        #endregion // Overrides / Event Handlers
    }
}
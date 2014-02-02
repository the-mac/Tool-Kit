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

#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
using System.Windows;
using Point = System.Windows.Point;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Point = Windows.Foundation.Point;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

using GART.Data;

#if X3D
using GART.X3D;
using Vector3 = GART.X3D.Vector3;
using Matrix = GART.X3D.Matrix;
using Viewport = GART.X3D.Viewport;
#else
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Viewport = Microsoft.Xna.Framework.Graphics.Viewport;
#endif

using System;
using GART.BaseControls;



namespace GART.Controls
{
    [TemplatePart(Name = WorldView.PartNames.View, Type = typeof(Canvas))]
    public class WorldView : ARItemsView
    {
        #region Static Version
        #region Part Names
        static internal class PartNames
        {
            public const string View = "View";
        }
        #endregion // Part Names

        #region Constants
        #endregion // Constants

        #endregion // Static Version

        #region Instance Version
        #region Member Variables
        Viewport viewport;
        Matrix projection;
        Matrix view;
        ControlOrientation previousOrientation;
        ControlOrientation currentOrientation;
        private double nearClippingPlane = 1;
        private double farClippingPlane = 275; // Just over 300 yards
        private bool viewPortNeedsRebuilding = true;
        private double minItemScale = 0.1;
        private double maxItemScale = 1.0;

        #endregion // Member Variables

        #region Constructors
        public WorldView()
        {
            DefaultStyleKey = typeof(WorldView);

            #if WIN_RT
            this.SizeChanged += (s, a) =>
            {
                RebuildViewport();
            };
            #endif
        }
        #endregion // Constructors

        #region Internal Methods
        private void EnsureViewport()
        {
            // Only do this once
            if (viewport.Width == 0 || previousOrientation != currentOrientation || viewPortNeedsRebuilding)
            {
                // Initialize the viewport and matrixes for 3d projection.
                // Create the ViewPort based on the size of ourselves (the view) and not the screen. That's 
                // becase the View should be sized to the camera preview, which is not the same size as 
                // the Page or Window.
                viewport = new Viewport(0, 0, (int)this.ActualWidth, (int)this.ActualHeight);
                viewPortNeedsRebuilding = false;

                float aspect = viewport.AspectRatio;
                projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, (float)NearClippingPlane, (float)FarClippingPlane);

                // Rotate items by adjusting camera's up vector
                Vector3 cameraUpVector = Vector3.Up;
                switch (currentOrientation)
                {
                    case ControlOrientation.Clockwise270Degrees:
                        cameraUpVector = Vector3.Right;
                        break;

                    case ControlOrientation.Clockwise90Degrees:
                        cameraUpVector = Vector3.Left;
                        break;

                    #if WIN_RT 
                    case ControlOrientation.Clockwise180Degrees:
                        cameraUpVector = Vector3.Down;
                        break;
                    #endif
                }

                view = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, cameraUpVector);

                previousOrientation = currentOrientation;
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers

        /// <summary>
        /// Occurs when <see cref="Orientation"/> property has changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            currentOrientation = (ControlOrientation)(e.NewValue);
            base.OnOrientationChanged(e);
        }
        
        
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            WorldViewItem item = new WorldViewItem();
            if (this.ItemContainerStyle != null)
            {
                item.Style = this.ItemContainerStyle;
            }
            return item;
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Attitude"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected override void OnAttitudeChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnAttitudeChanged(e);

            // Ensure that the viewport has been created
            EnsureViewport();

            // Get the RotationMatrix from the MotionReading.
            // Rotate it 90 degrees around the X axis to put it in the XNA Framework coordinate system.
            Matrix xnaAttitude = Matrix.CreateRotationX(MathHelper.PiOver2) * Attitude;

            // Loop through our items
            for (int i = 0; i < Items.Count; i++)
            {
                // Get the WorldVIewItem that matches the index
                WorldViewItem wvItem = ItemContainerGenerator.ContainerFromIndex(i) as WorldViewItem;

                // If we couldn't get a WorldViewItem for the index, skip
                if (wvItem == null) { continue; }

                // Get the ARItem that the WorldViewItem represents
                ARItem arItem = (ARItem)wvItem.DataContext;

                // Create a World matrix for the ARItems WorldLocation
                Matrix world = Matrix.CreateWorld(arItem.WorldLocation, new Vector3(0, 0, 1), new Vector3(0, 1, 0));

                // Use Viewport.Project to project the ARItems location in 3D space into screen coordinates
                Vector3 projected = viewport.Project(Vector3.Zero, projection, view, world * xnaAttitude);

                // If the point is outside of this range, it is behind the camera
                if (projected.Z > 1 || projected.Z < 0)
                {
                    // Out of range, just hide
                    wvItem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // In range so show
                    wvItem.Visibility = Visibility.Visible;

                    /*
                    // Create a TranslateTransform to position the WorldViewItem
                    TranslateTransform tt = new TranslateTransform();

                    // Offset by half of the WorldViewItems size to center it on the point
                    tt.X = projected.X - (wvItem.ActualWidth / 2);
                    tt.Y = projected.Y - (wvItem.ActualHeight / 2);

                    // Set the transform, which moves the item
                    wvItem.RenderTransform = tt;
                     */

                    // Create a CompositeTransform to position and scale the WorldViewItem
                    CompositeTransform ct = new CompositeTransform();

                    // Offset by half of the WorldViewItems size to center it on the point
					
					// TODO: Expose vertical limit property
					#if WIN_RT
					ct.TranslateX = projected.X - (wvItem.ActualWidth / 2) - (this.ActualWidth / 2);

                    // Ricky; Keep the y value within the screen
                    double y = projected.Y - (wvItem.ActualHeight / 2);
                    double h2 = this.Height / 2;
                    y = (y < -h2) ? -h2 : ((y > h2) ? h2 : y);
                    ct.TranslateY = y;

					#endif 
					
					#if WINDOWS_PHONE

                    ct.TranslateX = projected.X - (wvItem.ActualWidth / 2);
                    ct.TranslateY = projected.Y - (wvItem.ActualHeight / 2);

					#endif


                    #if WP7
                    double scale = MathHelper.Lerp((float)MinItemScale, (float)MaxItemScale, ((float)FarClippingPlane - Math.Abs(arItem.WorldLocation.Z)) / (float)FarClippingPlane);
                    #else
					double scale = MathHelper.Lerp(MinItemScale, MaxItemScale, (FarClippingPlane - Math.Abs(arItem.WorldLocation.Z)) / FarClippingPlane);
                    #endif
                    ct.ScaleX = scale;
                    ct.ScaleY = scale;

                    // Set the transform, which moves the item
                    wvItem.RenderTransform = ct;

                    // Set the items z-index so that the closest item is always rendered on top
                    Canvas.SetZIndex(wvItem, (int)(scale * 255));
                }
            }
        }
        #endregion // Overrides / Event Handlers

        #region Overridables / Event Triggers
        #endregion // Overridables / Event Triggers

        #region Public Methods

        /// <summary>
        /// Triggers the rebuild of viewport
        /// </summary>
        public void RebuildViewport()
        {
            InvalidateViewport();
            EnsureViewport();
        }

        /// <summary>
        /// Invalidates the viewport. A rebuild will be triggered during next EnsureViewport() call
        /// </summary>
        public void InvalidateViewport()
        {
            viewPortNeedsRebuilding = true;
        }

        /// <summary>
        /// Converts the screen coordinates to world coordiantes using the specified attitude matrix.
        /// </summary>
        /// <param name="pointOnScreen">
        /// The point in screen coordinates to translate.
        /// </param>
        /// <param name="attitude">
        /// The attitude matrix to use for translations.
        /// </param>
        /// <returns>
        /// A <see cref="Vector3"/> in world space.
        /// </returns>
        public Vector3 ScreenToWorld(Point pointOnScreen, Matrix attitude)
        {
            // Ensure that the viewport has been created
            EnsureViewport();

            // Use the attitude Matrix saved in the OnMouseLeftButtonUp handler.
            // Rotate it 90 degrees around the X axis to put it in the XNA Framework coordinate system.
            attitude = Matrix.CreateRotationX(MathHelper.PiOver2) * attitude;


            // for the near point we specified zero as the z component of the point which means this point is as close as 
            // possible to the camera, while one for the second point means it’s as far as possible from the camera
            Vector3 nearPoint = new Vector3((float)pointOnScreen.X, (float)pointOnScreen.Y, 0);
            Vector3 farPoint = new Vector3((float)pointOnScreen.X, (float)pointOnScreen.Y, 1);

            // Convert the near and far points from mouse space to 3D space
            nearPoint = viewport.Unproject(nearPoint, projection, view, attitude);
            farPoint = viewport.Unproject(farPoint, projection, view, attitude);

            // Now that we have the near and far points in 3D space, create a directional vector between the two points
            // and normalize it. Normalizing makes the vector only one unit in length but still pointing in the same 
            // direction.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // Now we take our direction and multiply it by 10, which essentially just moves it out almost to the edge of 
            // our sceeen (our edge is 12 as defined by FarClippingPlane above). 10 units is also where all the labels like
            // 'front', 'back', 'left', etc. are positioned. This way if the user taps on one of the labels, their new label
            // will be at the same location and will move in sync.
            Vector3 unprojected = direction * 10;

            // Return the unprojected location
            return unprojected;
        }

        /// <summary>
        /// Converts the screen coordinates to world coordiantes using the current attitude from motion services.
        /// </summary>
        /// <param name="pointOnScreen">
        /// The point in screen coordinates to translate.
        /// </param>
        /// <returns>
        /// A <see cref="Vector3"/> in world space.
        /// </returns>
        public Vector3 ScreenToWorld(Point pointOnScreen)
        {
            // Use version with specified attitude and pass current attitude
            return ScreenToWorld(pointOnScreen, Attitude);
        }
        #endregion // Public Methods

        #region Public Properties

        /// <summary>
        /// Sets the near clipping plane distance (in meters). Must me >= 0.0 and less than <see cref="FarClippingPlane"/>.
        /// Invalidates the viewport.
        /// </summary>
        public double NearClippingPlane
        {
            get { return nearClippingPlane; }
            set
            {
                if (value != nearClippingPlane && value < FarClippingPlane && value >= 0.0)
                {
                    nearClippingPlane = value;
                    InvalidateViewport();
                }
            }
        }

        /// <summary>
        /// Sets the far clipping plane distance (in meters). Must be > than <see cref="NearClippingPlane"/>.
        /// Invalidates the viewport.
        /// </summary>
        public double FarClippingPlane
        {
            get { return farClippingPlane; }
            set
            {
                if (value != farClippingPlane && value > NearClippingPlane)
                {
                    farClippingPlane = value;
                    InvalidateViewport();
                }
            }
        }

        /// <summary>
        /// Sets the default value for lower end of the scaling objects. Default is 0.1f
        /// </summary>
        public double MinItemScale
        {
            get { return minItemScale; }
            set
            {
                if (value != minItemScale && value < MaxItemScale && value >= 0.0)
                {
                    minItemScale = value;
                }
            }
        }

        public double MaxItemScale
        {
            get { return maxItemScale; }
            set
            {
                if (value != maxItemScale && value > MinItemScale)
                {
                    maxItemScale = value;
                }
            }
        }




        #endregion // Public Properties

        #endregion // Instance Version
    }
}

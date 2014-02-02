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
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
#else
using Windows.UI.Xaml;
#endif

using GART.BaseControls;
using System.ComponentModel;

namespace GART.Controls
{
    /// <summary>
    /// Defines the sources for calculating rotation.
    /// </summary>
    public enum RotationSource
    {
        /// <summary>
        /// Rotation will be equal to the <see cref="IARView.AttitudeHeading"/> property.
        /// </summary>
        AttitudeHeading,
        /// <summary>
        /// Rotation will be equal to the <see cref="IARView.TravelHeading"/> property.
        /// </summary>
        TravelHeading,
        /// <summary>
        /// Rotation will be set to North (0 degrees + offset).
        /// </summary>
        North,
        /// <summary>
        /// Rotation will not be calculated and must be manually specified.
        /// </summary>
        None
    }

    /// <summary>
    /// The base class for an augmented reality view that rotates a UI element.
    /// </summary>
    public abstract class ARRotateView : ARView
    {
        #region Static Version

        ControlOrientation currentOrientation = ControlOrientation.Default;

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="InvertRotation"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty InvertRotationProperty = DependencyProperty.Register("InvertRotation", typeof(bool), typeof(ARRotateView), new PropertyMetadata(false, OnInvertRotationChanged));

        private static void OnInvertRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARRotateView)d).OnInvertRotationChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="Rotation"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", typeof(double), typeof(ARRotateView), new PropertyMetadata(0d, OnRotationChanged));

        private static void OnRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARRotateView)d).OnRotationChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="RotationSource"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty RotationSourceProperty = DependencyProperty.Register("RotationSource", typeof(RotationSource), typeof(ARRotateView), new PropertyMetadata(RotationSource.AttitudeHeading, OnRotationSourceChanged));

        private static void OnRotationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARRotateView)d).OnRotationSourceChanged(e);
        }

        #endregion // Dependency Properties
        #endregion // Static Version

        #region Instance Version
        #region Internal Methods
        /// <summary>
        /// Forces an update by calling the <see cref="OnApplyRotation"/> method with the current rotation angle.
        /// </summary>
        private void UpdateRotation()
        {
            // If rotation source is None, bail
            if (RotationSource == RotationSource.None) { return; }

            // Figure out our source angle
            double angle = 0d;

            switch (RotationSource)
            {
                case RotationSource.AttitudeHeading:
                    angle = AttitudeHeading;
                    break;
                case RotationSource.TravelHeading:
                    angle = TravelHeading;
                    break;
                case RotationSource.North:
                    angle = 0d;
                    break;
                default:
                    break;
            }


            switch (currentOrientation)
            {
                case ControlOrientation.Clockwise270Degrees:
                    angle += 90;
                    break;

                case ControlOrientation.Clockwise90Degrees:
                    angle -= 90;
                    break;
            }

            // Assign
            Rotation = angle;
        }
        #endregion // Internal Methods

        #region Overridables / Event Triggers
        /// <summary>
        /// Occurs when the value of the <see cref="AttitudeHeading"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected override void OnAttitudeHeadingChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnAttitudeHeadingChanged(e);

            // Update
            if (RotationSource == RotationSource.AttitudeHeading)
            {
                UpdateRotation();
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Orientation" /> property has changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            currentOrientation = (ControlOrientation)(e.NewValue);
        }

        /// <summary>
        /// Occurs when the value of the <see cref="InvertRotation"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnInvertRotationChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateRotation();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Rotation"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnRotationChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="RotationSource"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnRotationSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateRotation();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="TravelHeading"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected override void OnTravelHeadingChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnTravelHeadingChanged(e);

            // Update
            if (RotationSource == RotationSource.TravelHeading)
            {
                UpdateRotation();
            }
        }
        #endregion // Overridables / Event Triggers

        #region Public Properties
        /// <summary>
        /// Gets or sets a value that indicates if <see cref="Rotation"/> should be calculated as the inverse of the <see cref="RotationSource"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Rotation"/> should be be calculated as the inverse of the <see cref="RotationSource"/>; otherwise <c>false</c>.
        /// </value>
        /// <remarks>
        /// Inverse means that the angle will be calculated as 360 - the source angle.
        /// </remarks>
        public bool InvertRotation
        {
            get
            {
                return (bool)GetValue(InvertRotationProperty);
            }
            set
            {
                SetValue(InvertRotationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Rotation of the <see cref="ARRotateView"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// The Rotation of the <see cref="ARRotateView"/>.
        /// </value>
        public double Rotation
        {
            get
            {
                return (double)GetValue(RotationProperty);
            }
            set
            {
                SetValue(RotationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the source of the rotation.
        /// </summary>
        /// <value>
        /// A <see cref="RotationSource"/> enum that indicates the source of the rotation.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public RotationSource RotationSource
        {
            get
            {
                return (RotationSource)GetValue(RotationSourceProperty);
            }
            set
            {
                SetValue(RotationSourceProperty, value);
            }
        }
        #endregion // Public Properties
        #endregion // Instance Version
    }
}

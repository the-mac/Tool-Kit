/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sdkBasicLensWP8CS.ViewModels;
using Windows.Phone.Media.Capture;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Microsoft.Devices;

namespace sdkBasicLensWP8CS
{
    public partial class Viewfinder : UserControl
    {
        private string frontFacingCameraButtonIconPath = "/Assets/appbar.frontfacingcamera.dark.rest.png";
        private string backFacingCameraButtonIconPath = "/Assets/appbar.frontfacingcamera.dark.down.png";
        private string flashButtonOffIconPath = "/Assets/camphotos.appbar.flash.off.rest.png";
        private string flashButtonAutoIconPath = "/Assets/camphotos.appbar.flash.auto.rest.png";
        private string flashButtonOnIconPath = "/Assets/camphotos.appbar.flash.on.rest.png";
        private string deleteButtonIconPath = "/Assets/appbar.delete.rest.png";
        private List<FlashState> desiredFlashStateOrder = new List<FlashState>() { FlashState.Auto, FlashState.Off, FlashState.On };
        
        private LensViewModel viewModel = new LensViewModel();

        private enum AppBarModeEnum { Empty, Viewfinder, MediaLibraryPhoto, IsolatedStoragePhoto }
        private AppBarModeEnum AppBarMode = AppBarModeEnum.Empty;
        private ApplicationBarIconButton frontFacingCameraButton;
        private ApplicationBarIconButton flashButton;
        private ApplicationBarIconButton deleteButton;
        private ApplicationBarMenuItem mediaLibraryPhotoMenuItemExample;
        private ApplicationBarMenuItem isolatedStoragePhotoMenuItemExample;

        private FrameworkElement livePreviewTapTarget;
        private FrameworkElement pointFocusBrackets;
        private Storyboard reviewImageSlideOff;
        private Storyboard pointFocusInProgress;
        private Storyboard pointFocusLocked;
        private Storyboard autoFocusInProgress;
        private Storyboard autoFocusLocked;
        private Storyboard rotateArrowButtonToPortrait;
        private Storyboard rotateArrowButtonToLandscape;
        private Storyboard displayFlashOn;
        private Storyboard displayFlashOff;
        private Storyboard displayFlashAuto;
        private CompositeTransform livePreviewTransform;
        private Image reviewImage;
        private EasingDoubleKeyFrame reviewImageSlideOffTranslateStart;
        private EasingDoubleKeyFrame reviewImageSlideOffTranslateEnd;
        private FrameworkElement leftArrowButtonPanel;

        public Viewfinder()
        {
            InitializeComponent();

            this.DataContext = this.viewModel;

            InitializeAppBar();

            UpdateFlashButtonState();
        }

        /// <summary>
        /// Tells the Viewfinder to update its UI to reflect a new PageOrientation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnOrientationChanged(PageOrientation newOrientation)
        {
            if (IsPortrait(newOrientation))
            {
                this.rotateArrowButtonToPortrait.Begin();
            }
            else
            {
                this.rotateArrowButtonToLandscape.Begin();
            }

            ApplyRotation();
        }

        #region Control event handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ReviewImageSlideOff.Completed += OnReviewImageSlideOffCompleted;
            this.LivePreviewTapTarget.Tap += OnViewfinderTap;
            ApplyRotation();
        }

        #endregion


        #region User event handlers

        private void OnShutterKeyPressed(object sender, EventArgs e)
        {
            if (this.MediaViewer.DisplayedElement == DisplayedElementType.Footer)
            {
                if ((this.viewModel.State == ViewModelState.Loaded) ||
                    (this.viewModel.State == ViewModelState.AutoFocusInProgress))
                {
                    this.viewModel.TakePhotoWithCameraButton();
                }
            }
            else
            {
                this.MediaViewer.JumpToFooter();
            }
        }

        private void OnShutterKeyReleased(object sender, EventArgs e)
        {
            this.viewModel.CancelAutoFocus();
        }

        private void OnShutterKeyHalfPressed(object sender, EventArgs e)
        {
            this.viewModel.BeginAutoFocusIfSupported();
        }

        private void OnLeftArrowButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Ensure this doesn't bubble down to the viewfinder and trigger a capture
            //
            e.Handled = true;

            if (this.MediaViewer.DragEnabled)
            {
                this.MediaViewer.ScrollLeftOneElement();
            }
        }

        private void OnViewfinderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.viewModel.State == ViewModelState.Loaded)
            {
                Point pointOnLivePreview = e.GetPosition(this.livePreviewTapTarget);
                Tracing.Trace("TAPPED AT: " + pointOnLivePreview);

                Point deltaFromCenter = new Point(
                    pointOnLivePreview.X - this.livePreviewTapTarget.ActualWidth / 2,
                    pointOnLivePreview.Y - this.livePreviewTapTarget.ActualHeight / 2);

                // Clamp delta to the actual size of the preview
                //
                Size previewSize = CalculateLivePreviewSize();
                deltaFromCenter.X = Math.Max(-1 * previewSize.Width / 2, deltaFromCenter.X);
                deltaFromCenter.X = Math.Min(previewSize.Width / 2, deltaFromCenter.X);
                deltaFromCenter.Y = Math.Max(-1 * previewSize.Height / 2, deltaFromCenter.Y);
                deltaFromCenter.Y = Math.Min(previewSize.Height / 2, deltaFromCenter.Y);

                // Move the focus brackets into position
                //
                CompositeTransform pointFocusBracketsTransform = (CompositeTransform)this.pointFocusBrackets.RenderTransform;
                pointFocusBracketsTransform.TranslateX = deltaFromCenter.X;
                pointFocusBracketsTransform.TranslateY = deltaFromCenter.Y;

                Point focusPointInScreenCoordinates = new Point(
                    (previewSize.Width / 2) + deltaFromCenter.X,
                    (previewSize.Height / 2) + deltaFromCenter.Y);

                // Convert focus point to coordinates in the preview resolution, which may be much larger and rotated
                //
                Size previewResolution = this.viewModel.PreviewResolution;
                double scaleFactor;
                Point focusPoint;
                if (IsPortrait(this.Orientation))
                {
                    scaleFactor = previewResolution.Height / previewSize.Width;
                    focusPoint = new Point(
                        focusPointInScreenCoordinates.Y * scaleFactor,
                        previewResolution.Height - (focusPointInScreenCoordinates.X * scaleFactor));
                }
                else
                {
                    scaleFactor = previewResolution.Width / previewSize.Width;
                    focusPoint = new Point(
                        focusPointInScreenCoordinates.X * scaleFactor,
                        focusPointInScreenCoordinates.Y * scaleFactor);
                }

                Tracing.Trace("FOCUSING AT: " + focusPoint);
                this.viewModel.BeginPointFocusAndCapture(focusPoint);
            }
        }

        private void OnFrontFacingCameraButtonClick(object sender, EventArgs e)
        {
            this.viewModel.ToggleBackFrontFacingCamera();
            EnableOrDisableViewfinderAppBarButtons();
            UpdateFlashButtonState();
        }

        #endregion

        #region MediaViewer event handlers

        private void MediaViewer_FooterDisplayed(object sender, EventArgs e)
        {
            LoadOrUnloadCamera();
            SetAppBarMode(AppBarModeEnum.Viewfinder);
        }

        private void MediaViewer_ItemDisplayed(object sender, ItemDisplayedEventArgs e)
        {
            LoadOrUnloadCamera();

            if (this.MediaViewer.Items[(int)e.ItemIndex] is MediaLibraryThumbnailedImage)
            {
                SetAppBarMode(AppBarModeEnum.MediaLibraryPhoto);
            }
            else if (this.MediaViewer.Items[(int)e.ItemIndex] is LocalFolderThumbnailedImage)
            {
                SetAppBarMode(AppBarModeEnum.IsolatedStoragePhoto);
            }
            else
            {
                SetAppBarMode(AppBarModeEnum.Empty);
            }
        }

        private void MediaViewer_ItemUnzoomed(object sender, EventArgs e)
        {
            if (this.ApplicationBar.IsVisible == false)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    RefreshAppBar();
                });
            }
        }

        private void MediaViewer_ItemZoomed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.ApplicationBar.IsVisible = false;
            });
        }

        #endregion

        #region ViewModel and storyboard event handlers

        private void OnCameraLoaded()
        {
            EnableOrDisableViewfinderAppBarButtons();
        }

        private void OnShowViewfinderChrome()
        {
            this.leftArrowButtonPanel.Visibility = Visibility.Visible;
            this.ApplicationBar.IsVisible = true;
            this.MediaViewer.DragEnabled = true;
        }

        private void OnHideViewfinderChrome()
        {
            this.leftArrowButtonPanel.Visibility = Visibility.Collapsed;
            this.ApplicationBar.IsVisible = false;
            this.MediaViewer.DragEnabled = false;
        }

        private void OnPointFocusStarted()
        {
            this.pointFocusInProgress.Begin();
        }

        private void OnPointFocusLocked()
        {
            //TODO: play auto focus locked sound?
            this.pointFocusInProgress.Stop();
            this.pointFocusLocked.Begin();
        }

        private void OnAutoFocusStarted()
        {
            this.autoFocusInProgress.Begin();
        }

        private void OnAutoFocusLocked()
        {
            //TODO: play auto focus locked sound?
            this.autoFocusInProgress.Stop();
            this.autoFocusLocked.Begin();
        }

        private void OnFocusCleared()
        {
            this.pointFocusInProgress.Stop();
            this.pointFocusLocked.Stop();
            this.autoFocusInProgress.Stop();
            this.autoFocusLocked.Stop();
        }

        private void OnStillCaptureComplete()
        {
            this.reviewImageSlideOff.Begin();
        }

        private void OnReviewImageAvailable()
        {
            if (this.viewModel.State == ViewModelState.PointFocusAndCaptureInProgress)
            {
                this.pointFocusLocked.Stop();
            }
            this.reviewImage.Visibility = Visibility.Visible;
        }

        private void OnReviewImageSlideOffCompleted(object sender, EventArgs e)
        {
            OnShowViewfinderChrome();
            this.reviewImage.Visibility = Visibility.Collapsed;
            this.reviewImageSlideOff.Stop();
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CameraSensorLocation":
                    {
                        UpdateFrontFacingCameraButtonState();
                        ApplyRotation();
                    } break;
                case "FlashState":
                    {
                        UpdateFlashButtonState();
                    } break;
            }
        }

        #endregion

        #region Helper methods

        private void LoadOrUnloadCamera()
        {
            switch (this.MediaViewer.DisplayedElement)
            {
                case DisplayedElementType.Footer:
                    {
                        this.viewModel.EnsureCameraLoaded();
                    } break;
                case DisplayedElementType.Item:
                    {
                        // Count how many positions away from the footer we are
                        //
                        int distance = this.MediaViewer.Items.Count - this.MediaViewer.DisplayedItemIndex;

                        if (distance == 1)
                        {
                            this.viewModel.EnsureCameraLoaded();
                        }
                        else if (distance == 2)
                        {
                            this.viewModel.UnloadCamera();
                            EnableOrDisableViewfinderAppBarButtons();
                        }
                    } break;
            }
        }

        private void ApplyRotation()
        {
            CompositeTransform reviewImageTransform = (CompositeTransform)this.reviewImage.RenderTransform;

            if (IsPortrait(this.Orientation))
            {
                this.livePreviewTransform.Rotation = this.viewModel.ViewfinderRotation;
                this.livePreviewTransform.CenterX = 400;
                this.livePreviewTransform.CenterY = 240;
                this.livePreviewTransform.TranslateX = -160;
                this.livePreviewTransform.TranslateY = 160;
            }
            else if (this.Orientation == PageOrientation.LandscapeRight)
            {
                this.livePreviewTransform.Rotation = 180;
                this.livePreviewTransform.CenterX = 400;
                this.livePreviewTransform.CenterY = 240;
                this.livePreviewTransform.TranslateX = 0;
                this.livePreviewTransform.TranslateY = 0;
            }
            else
            {
                this.livePreviewTransform.Rotation = 0;
                this.livePreviewTransform.CenterX = 400;
                this.livePreviewTransform.CenterY = 240;
                this.livePreviewTransform.TranslateX = 0;
                this.livePreviewTransform.TranslateY = 0;
            }

            // Update the slide off animation with the correct starting and ending points
            //
            this.reviewImageSlideOffTranslateStart.Value = livePreviewTransform.TranslateX;
            this.reviewImageSlideOffTranslateEnd.Value = reviewImageSlideOffTranslateStart.Value - 800;

            // Give the review image the same transformation as the live preview
            //
            reviewImageTransform.Rotation = this.livePreviewTransform.Rotation;
            reviewImageTransform.CenterX = this.livePreviewTransform.CenterX;
            reviewImageTransform.CenterY = this.livePreviewTransform.CenterY;
            reviewImageTransform.TranslateX = this.livePreviewTransform.TranslateX;
            reviewImageTransform.TranslateY = this.livePreviewTransform.TranslateY;

            // Apply a mirror effect for front facing camera
            //
            double scaleX = 1.0;
            if (this.viewModel.CameraSensorLocation == CameraSensorLocation.Front)
            {
                scaleX = -1.0;
            }
            this.livePreviewTransform.ScaleX = scaleX;
            reviewImageTransform.ScaleX = scaleX;

            // Special case for "left arrow" when rotated to LandscapeRight is needed so
            // that it doesn't appear under the app bar
            //
            if (this.Orientation == PageOrientation.LandscapeRight)
            {
                ((Storyboard)this.MediaViewer.Footer.FindName("MoveArrowButtonToLandscapeRightPosition")).Begin();
            }
        }

        private void InitializeAppBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            this.ApplicationBar = new ApplicationBar();
            this.ApplicationBar.Opacity = 0.9;

            this.frontFacingCameraButton = new ApplicationBarIconButton();
            this.frontFacingCameraButton.Text = AppResources.AppBarFrontFacingCameraText;
            this.frontFacingCameraButton.Click += OnFrontFacingCameraButtonClick;
            UpdateFrontFacingCameraButtonState();

            this.flashButton = new ApplicationBarIconButton();
            this.flashButton.Text = AppResources.AppBarFlashText;
            this.flashButton.Click += ToggleFlashState;

            this.deleteButton = new ApplicationBarIconButton();
            this.deleteButton.Text = AppResources.AppBarDeleteText;
            this.deleteButton.IconUri = new Uri(deleteButtonIconPath, UriKind.Relative);
            this.deleteButton.Click += DeletePhoto;

            this.mediaLibraryPhotoMenuItemExample = new ApplicationBarMenuItem(AppResources.AppBarMediaLibraryMenuItemSampleText);
            this.isolatedStoragePhotoMenuItemExample = new ApplicationBarMenuItem(AppResources.AppBarIsolatedStorageMenuItemSampleText);
        }

        private void SetAppBarMode(AppBarModeEnum newMode)
        {
            if (this.AppBarMode == newMode)
            {
                return;
            }
            this.AppBarMode = newMode;

            RefreshAppBar();
        }

        private void RefreshAppBar()
        {
            this.ApplicationBar.Buttons.Clear();
            this.ApplicationBar.MenuItems.Clear();
            switch (this.AppBarMode)
            {
                case AppBarModeEnum.Viewfinder:
                    {
                        this.ApplicationBar.IsVisible = true;

                        this.ApplicationBar.Buttons.Add(this.flashButton);
                        if (this.viewModel.FrontFacingPhotoCameraSupported)
                        {
                            this.ApplicationBar.Buttons.Add(this.frontFacingCameraButton);
                        }
                    } break;
                case AppBarModeEnum.MediaLibraryPhoto:
                    {
                        this.ApplicationBar.IsVisible = true;

                        this.ApplicationBar.MenuItems.Add(this.mediaLibraryPhotoMenuItemExample);
                    } break;
                case AppBarModeEnum.IsolatedStoragePhoto:
                    {
                        this.ApplicationBar.IsVisible = true;

                        this.ApplicationBar.Buttons.Add(this.deleteButton);
                        this.ApplicationBar.MenuItems.Add(this.isolatedStoragePhotoMenuItemExample);
                    } break;
                default:
                    {
                        this.ApplicationBar.IsVisible = false;
                    } break;
            }
        }

        private Size CalculateLivePreviewSize()
        {
            Size previewResolution = this.viewModel.PreviewResolution;
            Size boundingBox = new Size(this.livePreviewTapTarget.ActualWidth, this.livePreviewTapTarget.ActualHeight);

            if (IsPortrait(this.Orientation))
            {
                previewResolution = SwapHeightAndWidth(previewResolution);
            }

            double scale = Math.Min(boundingBox.Width / previewResolution.Width, boundingBox.Height / previewResolution.Height);

            Size previewSize = new Size(
                previewResolution.Width * scale,
                previewResolution.Height * scale);

            return previewSize;
        }

        private Size SwapHeightAndWidth(Size originalSize)
        {
            return new Size(originalSize.Height, originalSize.Width);
        }

        private bool IsPortrait(PageOrientation orientation)
        {
            return ((orientation == PageOrientation.Portrait) ||
                    (orientation == PageOrientation.PortraitUp) ||
                    (orientation == PageOrientation.PortraitDown));
        }

        private void EnableOrDisableViewfinderAppBarButtons()
        {
            EnableOrDisableFlashButton();
            this.frontFacingCameraButton.IsEnabled = this.viewModel.State == ViewModelState.Loaded;
        }

        private void UpdateFrontFacingCameraButtonState()
        {
            string iconPath;

            if (this.viewModel.CameraSensorLocation == CameraSensorLocation.Back)
            {
                iconPath = frontFacingCameraButtonIconPath;
            }
            else
            {
                iconPath = backFacingCameraButtonIconPath;
            }

            this.frontFacingCameraButton.IconUri = new Uri(iconPath, UriKind.Relative);
        }

        private void UpdateFlashButtonState()
        {
            EnableOrDisableFlashButton();

            switch ((FlashState)this.viewModel.FlashState)
            {
                case FlashState.Off:
                    {
                        this.flashButton.IconUri = new Uri(flashButtonOffIconPath, UriKind.Relative);
                    } break;
                case FlashState.On:
                    {
                        this.flashButton.IconUri = new Uri(flashButtonOnIconPath, UriKind.Relative);
                    } break;
                case FlashState.Auto:
                    {
                        this.flashButton.IconUri = new Uri(flashButtonAutoIconPath, UriKind.Relative);
                    } break;
            }
        }

        private void EnableOrDisableFlashButton()
        {
            this.flashButton.IsEnabled = ((this.viewModel.State == ViewModelState.Loaded) &&
                                          (this.viewModel.AvailableFlashStates.Count > 1));
        }

        private void DisplayNewFlashStateText()
        {
            switch ((FlashState)this.viewModel.FlashState)
            {
                case FlashState.Off:
                    {
                        this.displayFlashOff.Begin();
                        this.displayFlashOn.Stop();
                        this.displayFlashAuto.Stop();
                    } break;
                case FlashState.On:
                    {
                        this.displayFlashOff.Stop();
                        this.displayFlashOn.Begin();
                        this.displayFlashAuto.Stop();
                    } break;
                case FlashState.Auto:
                    {
                        this.displayFlashOff.Stop();
                        this.displayFlashOn.Stop();
                        this.displayFlashAuto.Begin();
                    } break;
            }
        }

        private void ToggleFlashState(object sender, EventArgs e)
        {
            FlashState currentFlashState = this.viewModel.FlashState;
            int currentIndexInOrderedList = this.desiredFlashStateOrder.IndexOf(currentFlashState);
            int newIndex = (currentIndexInOrderedList + 1) % this.desiredFlashStateOrder.Count;

            while (this.viewModel.AvailableFlashStates.Contains<FlashState>(this.desiredFlashStateOrder[newIndex]) == false)
            {
                newIndex = (newIndex + 1) % this.desiredFlashStateOrder.Count;
            }

            this.viewModel.FlashState = this.desiredFlashStateOrder[newIndex];

            DisplayNewFlashStateText();
        }

        private void DeletePhoto(object sender, EventArgs e)
        {
            this.viewModel.DeletePhoto(this.MediaViewer.DisplayedItemIndex);
        }

        #endregion
    }
}

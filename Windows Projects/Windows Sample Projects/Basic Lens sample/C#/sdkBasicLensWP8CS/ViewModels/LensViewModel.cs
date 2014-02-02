/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using sdkBasicLensWP8CS.Models;
using sdkBasicLensWP8CS.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Phone.Media.Capture;

namespace sdkBasicLensWP8CS.ViewModels
{
    public enum ViewModelState { Unloaded, Loading, Loaded, AutoFocusInProgress, PointFocusAndCaptureInProgress, AutoFocusAndCaptureInProgress, CaptureInProgress }

    class LensViewModel : DependencyObject, INotifyPropertyChanged, ICameraEngineEvents
    {
        // Our convention is that we will store the thumbnail of a file named foo.jpg as
        // foo.jpg.thumb.jpg
        //
        private const string thumbnailSuffix = ".thumb.jpg";
        private const string photosDirectoryName = "Photos";

        #region Private fields

        private MediaLibrary mediaLibrary = new MediaLibrary();
        private CameraEngine cameraEngine;
        private CameraSensorLocation cameraSensorLocation = CameraSensorLocation.Back;
        public ViewModelState State { get; private set; }
        private Dictionary<CameraSensorLocation, CameraSettings> cameraSettings = new Dictionary<CameraSensorLocation,CameraSettings>();

        #endregion

        #region Public events

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action CameraLoaded;
        public event Action StillCaptureComplete;
        public event Action ReviewImageAvailable;
        public event Action AutoFocusStarted;
        public event Action PointFocusStarted;
        public event Action AutoFocusLocked;
        public event Action PointFocusLocked;
        public event Action FocusCleared;
        public event Action ShowViewfinderChrome;
        public event Action HideViewfinderChrome;

        #endregion

        #region Public properties

        public Size PreviewResolution
        {
            get
            {
                Windows.Foundation.Size wfSize = this.cameraEngine.PreviewResolution;
                return new Size(wfSize.Width, wfSize.Height);
            }
        }

        public PageOrientation Orientation
        {
            get
            {
                return this.cameraEngine.Orientation;
            }
            set
            {
                this.cameraEngine.Orientation = value;
            }
        }

        public CameraSensorLocation CameraSensorLocation
        {
            get
            {
                return this.cameraSensorLocation;
            }
            private set
            {
                this.cameraSensorLocation = value;
                OnPropertyChanged("CameraSensorLocation");
            }
        }

        public static readonly DependencyProperty CameraRollProperty = DependencyProperty.Register(
            "CameraRoll",
            typeof(ObservableCollection<object>),
            typeof(LensViewModel),
            new PropertyMetadata(new ObservableCollection<object>(), OnCameraRollChanged));

        public ObservableCollection<object> CameraRoll
        {
            get { return (ObservableCollection<object>)GetValue(CameraRollProperty); }
            set { SetValue(CameraRollProperty, value); }
        }

        public bool FrontFacingPhotoCameraSupported
        {
            get
            {
                return this.cameraEngine.IsFrontFacingPhotoCameraSupported;
            }
        }

        public static readonly DependencyProperty PreviewBrushProperty = DependencyProperty.Register(
            "PreviewBrush",
            typeof(VideoBrush),
            typeof(LensViewModel),
            null);

        public VideoBrush PreviewBrush
        {
            get { return (VideoBrush)GetValue(PreviewBrushProperty); }
            set { SetValue(PreviewBrushProperty, value); }
        }

        public static readonly DependencyProperty ViewfinderRotationProperty = DependencyProperty.Register(
            "ViewfinderRotation",
            typeof(double),
            typeof(LensViewModel), 
            new PropertyMetadata(90.0));

        public double ViewfinderRotation
        {
            get { return (double)GetValue(ViewfinderRotationProperty); }
            set { SetValue(ViewfinderRotationProperty, value); }
        }

        public static readonly DependencyProperty ReviewImageBitmapProperty = DependencyProperty.Register(
            "ReviewImageBitmap",
            typeof(WriteableBitmap),
            typeof(LensViewModel),
            null);

        public WriteableBitmap ReviewImageBitmap
        {
            get { return (WriteableBitmap)GetValue(ReviewImageBitmapProperty); }
            set { SetValue(ReviewImageBitmapProperty, value); }
        }

        public IReadOnlyList<FlashState> AvailableFlashStates
        {
            get
            {
                return this.cameraEngine.GetAvailableFlashStates(this.CameraSensorLocation);
            }
        }

        public FlashState FlashState
        {
            get
            {
                return this.cameraSettings[this.CameraSensorLocation].FlashState;
            }
            set
            {
                this.cameraSettings[this.CameraSensorLocation].FlashState = value;

                // Apply it immediately if the camera is loaded
                //
                if (this.State != ViewModelState.Unloaded)
                {
                    this.cameraEngine.FlashState = value;
                }

                OnPropertyChanged("FlashState");
            }
        }

        public bool IsFocusSupported
        {
            get
            {
                return this.cameraEngine.IsFocusSupported;
            }
        }

        public bool IsFocusRegionSupported
        {
            get
            {
                return this.cameraEngine.IsFocusRegionSupported;
            }
        }

        public bool CameraRollIsNotEmpty
        {
            get
            {
                if (this.CameraRoll == null)
                {
                    return false;
                }
                return this.CameraRoll.Count > 0;
            }
        }

        public bool IsShutterSoundEnabledByUser
        {
            get
            {
                return this.cameraEngine.IsShutterSoundEnabledByUser;
            }
        }

        #endregion

        public LensViewModel()
        {
            this.State = ViewModelState.Unloaded;
            this.cameraEngine = new CameraEngine(this);
            InitializeCameraSettings();
            CopySampleFilesToIsolatedStorage();
        }

        #region Camera Event Handlers

        public void OnCameraLoaded(ICameraCaptureDevice captureDevice)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (this.State == ViewModelState.Loading)
                {
                    try
                    {
                        this.PreviewBrush = new VideoBrush();
                        this.PreviewBrush.Stretch = Stretch.Uniform;
                        this.PreviewBrush.SetSource(captureDevice);
                        this.ViewfinderRotation = this.cameraEngine.ViewfinderRotation;

                        ApplyCameraSettings();

                        this.ReviewImageBitmap = new WriteableBitmap(
                            (int)captureDevice.PreviewResolution.Width,
                            (int)captureDevice.PreviewResolution.Height);

                        this.State = ViewModelState.Loaded;
                        CameraLoaded();
                    }
                    catch (ObjectDisposedException)
                    {
                        // It's possible that an unload operation was queued up and executed 
                        // after this event was raised since it was dispatched over to the UI
                        // thread, so it is possible for the captureDevice to be disposed.
                    }
                }
            });
        }

        public void OnFocusComplete(bool succeeded)
        {
            if (succeeded)
            {
                switch (this.State)
                {
                    case ViewModelState.AutoFocusAndCaptureInProgress:
                        {
                            Dispatcher.BeginInvoke(AutoFocusLocked);
                        } break;
                    case ViewModelState.AutoFocusInProgress:
                        {
                            Dispatcher.BeginInvoke(AutoFocusLocked);
                            this.State = ViewModelState.Loaded;
                        } break;
                    case ViewModelState.PointFocusAndCaptureInProgress:
                        {
                            Dispatcher.BeginInvoke(PointFocusLocked);
                        } break;
                }
            }
            else
            {
                Dispatcher.BeginInvoke(FocusCleared);
                this.State = ViewModelState.Loaded;
            }
        }

        public void OnReviewImageAvailable()
        {
            if (this.State == ViewModelState.PointFocusAndCaptureInProgress)
            {
                this.cameraEngine.ResetFocus();
            }
            Dispatcher.BeginInvoke(() =>
            {
                ReviewImageAvailable();
            });
        }

        public void OnStillCaptureComplete(Stream thumbnailStream, Stream imageStream)
        {
            imageStream.Seek(0, SeekOrigin.Begin);
            Picture newPicture = this.mediaLibrary.SavePictureToCameraRoll(GeneratePhotoName(), imageStream);

            this.State = ViewModelState.Loaded;

            Dispatcher.BeginInvoke(() =>
            {
                this.CameraRoll.Add(new MediaLibraryThumbnailedImage(newPicture));
                StillCaptureComplete();
            });
        }

        #endregion

        #region Public Methods

        public void LoadCameraRoll()
        {
            List<IThumbnailedImage> tempCameraRoll = new List<IThumbnailedImage>();

            // Lenses don't typically display the entire camera roll, only photos captured during the 
            // current session.  Uncomment the following line to display the entire camera roll anyway.
            //
            //LoadFromMediaLibrary(tempCameraRoll);

            LoadFromIsolatedStorage(tempCameraRoll);

            tempCameraRoll.Sort(new ThumbnailedImageDateComparer());

            this.CameraRoll = new ObservableCollection<object>(tempCameraRoll);
        }

        public void EnsureCameraLoaded()
        {
            if (this.State == ViewModelState.Unloaded)
            {
                this.State = ViewModelState.Loading;
                this.cameraEngine.LoadCamera(
                    cameraSensorLocation,
                    this.cameraSettings[this.CameraSensorLocation].CaptureResolution);
            }
        }

        public void UnloadCamera()
        {
            this.State = ViewModelState.Unloaded;
            this.cameraEngine.UnloadCamera();
            this.PreviewBrush = null;
        }

        public void TakePhotoWithCameraButton()
        {
            if ((this.State == ViewModelState.Loaded) ||
                (this.State == ViewModelState.AutoFocusInProgress))
            {
                HideViewfinderChrome();
                BeginStillCapture();
                this.State = ViewModelState.CaptureInProgress;
            }
        }

        public void BeginAutoFocusIfSupported()
        {
            if ((this.cameraEngine.IsFocusSupported) && 
                (this.State == ViewModelState.Loaded))
            {
                HideViewfinderChrome();
                AutoFocusStarted();
                this.State = ViewModelState.AutoFocusInProgress;
                this.cameraEngine.BeginFocus(null);
            }
        }

        public void BeginPointFocusAndCapture(Point focusPoint)
        {
            if (this.State == ViewModelState.Loaded)
            {
                HideViewfinderChrome();

                if (this.cameraEngine.IsFocusRegionSupported)
                {
                    PointFocusStarted();
                    this.State = ViewModelState.PointFocusAndCaptureInProgress;
                    this.cameraEngine.BeginFocus(new Windows.Foundation.Point(focusPoint.X, focusPoint.Y));
                }
                else if (this.cameraEngine.IsFocusSupported)
                {
                    AutoFocusStarted();
                    this.State = ViewModelState.AutoFocusAndCaptureInProgress;
                    this.cameraEngine.BeginFocus(null);
                }
                else
                {
                    this.State = ViewModelState.CaptureInProgress;
                }

                BeginStillCapture();
            }
        }

        public void CancelAutoFocus()
        {
            if (this.State == ViewModelState.AutoFocusInProgress)
            {
                this.cameraEngine.CancelFocus();
                this.State = ViewModelState.Loaded;
            }
            if (this.State == ViewModelState.Loaded)
            {
                ShowViewfinderChrome();
            }
            FocusCleared();
            this.cameraEngine.ResetFocus();
        }

        public void ToggleBackFrontFacingCamera()
        {
            if (this.CameraSensorLocation == CameraSensorLocation.Back)
            {
                this.CameraSensorLocation = CameraSensorLocation.Front;
            }
            else
            {
                this.CameraSensorLocation = CameraSensorLocation.Back;
            }

            UnloadCamera();
            EnsureCameraLoaded();
        }

        public void DeletePhoto(int index)
        {
            LocalFolderThumbnailedImage image = this.CameraRoll[index] as LocalFolderThumbnailedImage;
            if (image == null)
            {
                throw new ArgumentException();
            }

            string fileName = image.ImageFileName;
            string thumbName = image.ThumbnailFileName;
            image = null;

            this.CameraRoll.RemoveAt((int)index);

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                storage.DeleteFile(fileName);
                storage.DeleteFile(thumbName);
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Initializes the CameraSettings objects for the back and front facing cameras with default values.
        /// </summary>
        private void InitializeCameraSettings()
        {
            this.cameraSettings.Add(CameraSensorLocation.Back, GetDefaultCameraSettings(CameraSensorLocation.Back));

            if (this.cameraEngine.IsFrontFacingPhotoCameraSupported)
            {
                this.cameraSettings.Add(CameraSensorLocation.Front, GetDefaultCameraSettings(CameraSensorLocation.Front));
            }
        }
        
        private CameraSettings GetDefaultCameraSettings(CameraSensorLocation sensorLocation)
        {
            CameraSettings settings = new CameraSettings();

            IReadOnlyList<FlashState> availableFlashStates = this.cameraEngine.GetAvailableFlashStates(sensorLocation);
            if (availableFlashStates.Count == 0)
            {
                settings.FlashState = FlashState.Off;
            }
            else if (availableFlashStates.Contains<FlashState>(FlashState.Auto))
            {
                settings.FlashState = FlashState.Auto;
            }
            else
            {
                settings.FlashState = availableFlashStates[0];
            }

            IReadOnlyList<Windows.Foundation.Size> availableCaptureResolutions = this.cameraEngine.GetAvailablePhotoCaptureResolutions(sensorLocation);

            // Pick the largest as a default
            //
            settings.CaptureResolution = availableCaptureResolutions[0];

            return settings;
        }

        private void BeginStillCapture()
        {
            Stream thumbnailStream = new MemoryStream();
            Stream imageStream = new MemoryStream();
            this.cameraEngine.BeginPhotoCapture(this.ReviewImageBitmap.Pixels, thumbnailStream, imageStream);
        }

        private void LoadFromMediaLibrary(List<IThumbnailedImage> tempCameraRoll)
        {
            PictureAlbum rootAblum = this.mediaLibrary.RootPictureAlbum;

            foreach (PictureAlbum album in rootAblum.Albums)
            {
                if (album.Name == AppResources.CameraRollAlbumName)
                {
                    foreach (Picture picture in album.Pictures)
                    {
                        tempCameraRoll.Add(new MediaLibraryThumbnailedImage(picture));
                    }
                }
            }
        }

        private void LoadFromIsolatedStorage(List<IThumbnailedImage> tempCameraRoll)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string[] photoFileNames = store.GetFileNames(Path.Combine(photosDirectoryName, "*.*"));

                foreach (string photoFileName in photoFileNames)
                {
                    if (photoFileName.Contains(thumbnailSuffix) == false)
                    {
                        string thumbnailFileName = photoFileName + thumbnailSuffix;
                        string photoFileNameAndPath = Path.Combine(photosDirectoryName, photoFileName);
                        string thumbnailFileNameAndPath = Path.Combine(photosDirectoryName, thumbnailFileName);

                        // Make sure it has a thumbnail file before adding it
                        //
                        if (store.FileExists(thumbnailFileNameAndPath) == true)
                        {
                            tempCameraRoll.Add(new LocalFolderThumbnailedImage(photoFileNameAndPath, thumbnailFileNameAndPath));
                        }
                    }
                }
            }
        }

        private void CopySampleFilesToIsolatedStorage()
        {
            // Visual Studio deploys our sample photos to the installation directory of the app.
            // To better simulate real world usage of photos in isolated storage (e.g. you'll 
            // definitely need write access to the location they are stored in, and you don't have
            // write access to the installation folder), we'll copy them to isolated storage if we
            // haven't already.
            //

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.GetDirectoryNames().Contains<string>(photosDirectoryName))
                {
                    // They have already been copied, don't do anything
                    //
                    return;
                }

                store.CreateDirectory(photosDirectoryName);

                string[] photoFileNames = { "Photos\\Desert.jpg", "Photos\\Desert.jpg.thumb.jpg", "Photos\\Lighthouse.jpg", "Photos\\Lighthouse.jpg.thumb.jpg" };

                foreach(string photoFileName in photoFileNames)
                {
                    using (Stream input = Application.GetResourceStream(new Uri(photoFileName, UriKind.Relative)).Stream)
                    {
                        using (IsolatedStorageFileStream output = store.CreateFile(photoFileName))
                        {
                            byte[] readBuffer = new byte[4096];
                            int bytesRead = -1;

                            // Copy the file from the installation folder to isolated storage. 
                            while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                output.Write(readBuffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string GeneratePhotoName()
        {
            return DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".LensSample";
        }

        private void ApplyCameraSettings()
        {
            this.cameraEngine.FlashState = this.cameraSettings[this.CameraSensorLocation].FlashState;
        }

        private static void OnCameraRollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            LensViewModel viewModel = (LensViewModel)sender;

            if (e.OldValue != null)
            {
                ((ObservableCollection<object>)e.OldValue).CollectionChanged -= viewModel.LensViewModel_CollectionChanged;
            }
            if (e.NewValue != null)
            {
                ((ObservableCollection<object>)e.NewValue).CollectionChanged += viewModel.LensViewModel_CollectionChanged;
            }

            viewModel.OnPropertyChanged("CameraRollIsNotEmpty");
        }

        private void LensViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("CameraRollIsNotEmpty");
        }

        #endregion
    }
}

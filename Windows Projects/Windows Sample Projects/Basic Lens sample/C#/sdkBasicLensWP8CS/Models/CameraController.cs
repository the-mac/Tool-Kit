/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.Media.Capture;


namespace sdkBasicLensWP8CS.Models
{
    public class CameraController
    {
        private CameraCaptureSequence cameraCaptureSequence;
        private PageOrientation orienation;

        public PhotoCaptureDevice PhotoCaptureDevice { get; private set; }

        public PageOrientation Orientation
        {
            get
            {
                return this.orienation;
            }
            set
            {
                this.orienation = value;
                ApplyOrientation();
            }
        }

        public event Action<uint> FrameAcquired;
        
        public CameraSensorLocation CameraSensorLocation { get; set; }

        public Windows.Foundation.Size CaptureResolution
        {
            get
            {
                // Always return the largest resolution
                //
                return PhotoCaptureDevice.GetAvailableCaptureResolutions(this.CameraSensorLocation)[0];
            }
        }

        public double ViewfinderRotation
        {
            get
            {
                if (this.PhotoCaptureDevice != null)
                {
                    return (double)this.PhotoCaptureDevice.SensorRotationInDegrees;
                }
                else
                {
                    // Return our best guess if we haven't loaded the photoCaptureDevice yet
                    //
                    return 90;
                }
            }
        }

        public IReadOnlyList<Size> GetAvailablePhotoCaptureResolutions(CameraSensorLocation cameraSensorLocation)
        {
            return PhotoCaptureDevice.GetAvailableCaptureResolutions(cameraSensorLocation);
        }

        public IReadOnlyList<FlashState> GetAvailableFlashStates(CameraSensorLocation cameraSensorLocation)
        {
            IReadOnlyList<object> rawValueList = PhotoCaptureDevice.GetSupportedPropertyValues(cameraSensorLocation, KnownCameraPhotoProperties.FlashMode);
            List<FlashState> flashStates = new List<FlashState>(rawValueList.Count);
            
            foreach (object rawValue in rawValueList)
            {
                flashStates.Add((FlashState)(uint)rawValue);
            }

            return flashStates.AsReadOnly();
        }

        public FlashState FlashState
        {
            get
            {
                return (FlashState)(uint)this.PhotoCaptureDevice.GetProperty(KnownCameraPhotoProperties.FlashMode);
            }
            set
            {
                this.PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.FlashMode, value);
            }
        }

        public Size PreviewResolution
        {
            get
            {
                return this.PhotoCaptureDevice.PreviewResolution;
            }
        }

        public bool FrontFacingPhotoCameraSupported
        {
            get
            {
                return PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>(CameraSensorLocation.Front);
            }
        }

        public bool IsFocusRegionSupported
        {
            get
            {
                return PhotoCaptureDevice.IsFocusRegionSupported(this.CameraSensorLocation);
            }
        }

        public bool IsFocusSupported
        {
            get
            {
                return PhotoCaptureDevice.IsFocusSupported(this.CameraSensorLocation);
            }
        }

        public bool IsShutterSoundEnabledByUser
        {
            get
            {
                return (bool)PhotoCaptureDevice.GetProperty(KnownCameraGeneralProperties.IsShutterSoundEnabledByUser);
            }
        }

        public CameraController()
        {
            this.CameraSensorLocation = CameraSensorLocation.Back;
        }

        public async Task<bool> LoadPhotoCamera(CameraSensorLocation cameraSensorLocation, Size captureResolution)
        {
            // If camera is already loaded, ignore the request
            //
            if (this.PhotoCaptureDevice != null)
            {
                return false;
            }

            this.CameraSensorLocation = cameraSensorLocation;
            this.PhotoCaptureDevice = await PhotoCaptureDevice.OpenAsync(this.CameraSensorLocation, captureResolution);
            ApplyOrientation();

            this.PhotoCaptureDevice.SetProperty(KnownCameraGeneralProperties.PlayShutterSoundOnCapture, true);

            return true;
        }

        public void UnloadPhotoCamera()
        {
            // If the camera is already unloaded, ignore the request
            //
            if (this.PhotoCaptureDevice == null)
            {
                return;
            }

            this.PhotoCaptureDevice.Dispose();
            this.PhotoCaptureDevice = null;
        }

        public IAsyncOperation<CameraFocusStatus> BeginFocus(Point? focusPoint)
        {
            if (focusPoint != null)
            {
                this.PhotoCaptureDevice.FocusRegion = new Rect(focusPoint.Value.X, focusPoint.Value.Y, 0, 0);
            }
            else
            {
                this.PhotoCaptureDevice.FocusRegion = null;
            }
            return this.PhotoCaptureDevice.FocusAsync();
        }

        public async Task ResetFocus()
        {
            await this.PhotoCaptureDevice.ResetFocusAsync();
        }

        public async Task PrepareCameraCaptureSequence()
        {
            this.cameraCaptureSequence = this.PhotoCaptureDevice.CreateCaptureSequence(1);
            this.cameraCaptureSequence.FrameAcquired += cameraCaptureSequence_FrameAcquired;
            await this.PhotoCaptureDevice.PrepareCaptureSequenceAsync(this.cameraCaptureSequence);
        }

        private void cameraCaptureSequence_FrameAcquired(CameraCaptureSequence sender, FrameAcquiredEventArgs args)
        {
            FrameAcquired(args.Index);
        }

        public void GetReviewImage(uint frameIndex, int[] pixels)
        {
            this.cameraCaptureSequence.Frames[(int)frameIndex].GetPreviewBufferArgb(pixels);
        }

        public async Task BeginCameraCaptureSequence(Stream thumbnailStream, Stream imageStream)
        {
            this.cameraCaptureSequence.Frames[0].ThumbnailStream = thumbnailStream.AsOutputStream();
            this.cameraCaptureSequence.Frames[0].CaptureStream = imageStream.AsOutputStream();

            await this.cameraCaptureSequence.StartCaptureAsync();
        }

        private void ApplyOrientation()
        {
            int rotation = 0;

            if (this.CameraSensorLocation == CameraSensorLocation.Back)
            {
                switch (this.orienation)
                {
                    case PageOrientation.LandscapeLeft:
                        {
                            rotation = 0;
                        } break;
                    case PageOrientation.PortraitUp:
                        {
                            rotation = 90;
                        } break;
                    case PageOrientation.LandscapeRight:
                        {
                            rotation = 180;
                        } break;
                    case PageOrientation.PortraitDown:
                        {
                            rotation = 270;
                        } break;
                }
            }
            else if (this.CameraSensorLocation == CameraSensorLocation.Front)
            {
                switch (this.orienation)
                {
                    case PageOrientation.LandscapeLeft:
                        {
                            rotation = 0;
                        } break;
                    case PageOrientation.PortraitUp:
                        {
                            rotation = 270;
                        } break;
                    case PageOrientation.LandscapeRight:
                        {
                            rotation = 180;
                        } break;
                    case PageOrientation.PortraitDown:
                        {
                            rotation = 90;
                        } break;
                }
            }

            PhotoCaptureDevice photoCaptureDevice = this.PhotoCaptureDevice;
            if (photoCaptureDevice != null)
            {
                photoCaptureDevice.SetProperty(KnownCameraGeneralProperties.EncodeWithOrientation, rotation);
            }
        }

    }
}

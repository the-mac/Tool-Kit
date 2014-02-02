/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Windows.Foundation;
using Windows.Phone.Media.Capture;

namespace sdkBasicLensWP8CS.Models.Workflows
{
    class LoadCameraWorkflow : WorkflowBase
    {
        private Size captureResolution;
        private CameraSensorLocation cameraSensorLocation;

        public LoadCameraWorkflow(ICameraEngineEvents callback, CameraController cameraController, CameraSensorLocation cameraSensorLocation, Size captureResolution)
            : base(callback, cameraController)
        {
            this.cameraSensorLocation = cameraSensorLocation;
            this.captureResolution = captureResolution;
        }

        protected async override void DoWork()
        {
            bool succeeded = await this.cameraController.LoadPhotoCamera(this.cameraSensorLocation, this.captureResolution);

            if (succeeded)
            {
                await this.cameraController.PrepareCameraCaptureSequence();
                this.callback.OnCameraLoaded(this.cameraController.PhotoCaptureDevice);
            }
            OnComplete();
        }
    }
}

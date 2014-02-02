/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System.IO;

namespace sdkBasicLensWP8CS.Models.Workflows
{
    class PhotoCaptureWorkflow : WorkflowBase
    {
        private Stream thumbnailStream;
        private Stream imageStream;
        private int[] reviewImagePixels;

        public PhotoCaptureWorkflow(ICameraEngineEvents callback, CameraController cameraController, int[] reviewImagePixels, Stream thumbnailStream, Stream imageStream)
            : base(callback, cameraController)
        {
            this.reviewImagePixels = reviewImagePixels;
            this.thumbnailStream = thumbnailStream;
            this.imageStream = imageStream;
        }

        protected async override void DoWork()
        {
            this.cameraController.FrameAcquired += cameraController_FrameAcquired;

            await this.cameraController.BeginCameraCaptureSequence(this.thumbnailStream, this.imageStream);

            this.callback.OnStillCaptureComplete(this.thumbnailStream, this.imageStream);
            this.cameraController.FrameAcquired -= cameraController_FrameAcquired;
            OnComplete();
        }

        private void cameraController_FrameAcquired(uint frameIndex)
        {
            this.cameraController.GetReviewImage(frameIndex, this.reviewImagePixels);
            this.callback.OnReviewImageAvailable();
        }
    }
}

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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.Media.Capture;

namespace sdkBasicLensWP8CS.Models.Workflows
{
    class FocusWorkflow : WorkflowBase
    {
        private bool isCanceled = false;
        private Point? focusPoint;
        private IAsyncOperation<CameraFocusStatus> asyncOperation;
        private object masterLock = new object();

        public FocusWorkflow(ICameraEngineEvents callback, CameraController cameraController, Point? focusPoint)
            : base(callback, cameraController)
        {
            this.focusPoint = focusPoint;
        }

        protected async override void DoWork()
        {
            if (!isCanceled)
            {
                IAsyncOperation<CameraFocusStatus> asyncOperation = this.cameraController.BeginFocus(this.focusPoint);

                lock (this.masterLock)
                {
                    this.asyncOperation = asyncOperation;
                }

                try
                {
                    await this.asyncOperation.AsTask<CameraFocusStatus>();
                }
                catch (TaskCanceledException)
                {
                }

                this.callback.OnFocusComplete(!this.isCanceled);
            }
            OnComplete();
        }

        public void Cancel()
        {
            this.isCanceled = true;

            lock (this.masterLock)
            {
                if (this.asyncOperation != null)
                {
                    this.asyncOperation.Cancel();
                }
            }
        }
    }
}

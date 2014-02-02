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
using sdkBasicLensWP8CS.Models.Workflows;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Windows.Foundation;
using Windows.Phone.Media.Capture;

namespace sdkBasicLensWP8CS.Models
{
    class CameraEngine
    {
        private CameraController cameraController = new CameraController();

        private WorkflowBase currentWorkflow;
        private Queue<WorkflowBase> workflowQueue = new Queue<WorkflowBase>();
        private ICameraEngineEvents callback;

        public CameraEngine(ICameraEngineEvents callback)
        {
            this.callback = callback;
        }

        #region Public properties

        public double ViewfinderRotation
        {
            get
            {
                return this.cameraController.ViewfinderRotation;
            }
        }

        public bool IsFrontFacingPhotoCameraSupported
        {
            get
            {
                return this.cameraController.FrontFacingPhotoCameraSupported;
            }
        }

        public PageOrientation Orientation
        {
            get
            {
                return this.cameraController.Orientation;
            }
            set
            {
                this.cameraController.Orientation = value;
            }
        }

        public Size PreviewResolution
        {
            get
            {
                return this.cameraController.PreviewResolution;
            }
        }

        public FlashState FlashState
        {
            get
            {
                return this.cameraController.FlashState;
            }
            set
            {
                this.cameraController.FlashState = value;
            }
        }

        public bool IsFocusRegionSupported
        {
            get
            {
                return this.cameraController.IsFocusRegionSupported;
            }
        }

        public bool IsFocusSupported
        {
            get
            {
                return this.cameraController.IsFocusSupported;
            }
        }

        public bool IsShutterSoundEnabledByUser
        {
            get
            {
                return this.cameraController.IsShutterSoundEnabledByUser;
            }
        }

        #endregion

        #region Public methods

        public IReadOnlyList<FlashState> GetAvailableFlashStates(CameraSensorLocation cameraSensorLocation)
        {
            return this.cameraController.GetAvailableFlashStates(cameraSensorLocation);
        }

        public IReadOnlyList<Size> GetAvailablePhotoCaptureResolutions(CameraSensorLocation cameraSensorLocation)
        {
            return this.cameraController.GetAvailablePhotoCaptureResolutions(cameraSensorLocation);
        }

        public void LoadCamera(CameraSensorLocation cameraSensorLocation, Windows.Foundation.Size captureResolution)
        {
            EnqueueWorkflow(new LoadCameraWorkflow(this.callback, this.cameraController, cameraSensorLocation, captureResolution));
        }

        public void UnloadCamera()
        {
            EnqueueWorkflow(new UnloadCameraWorkflow(this.callback, this.cameraController));
        }

        public void BeginFocus(Point? focusPoint)
        {
            EnqueueWorkflow(new FocusWorkflow(this.callback, this.cameraController, focusPoint));
        }

        public void CancelFocus()
        {
            lock (this.workflowQueue)
            {
                if (this.currentWorkflow is FocusWorkflow)
                {
                    FocusWorkflow focusWorkflow = (FocusWorkflow)this.currentWorkflow;
                    focusWorkflow.Cancel();
                }

                foreach (WorkflowBase workflow in this.workflowQueue)
                {
                    if (workflow is FocusWorkflow)
                    {
                        FocusWorkflow focusWorkflow = (FocusWorkflow)workflow;
                        focusWorkflow.Cancel();
                    }
                }
            }
        }

        public void ResetFocus()
        {
            EnqueueWorkflow(new ResetFocusWorkflow(this.callback, this.cameraController));
        }

        public void BeginPhotoCapture(int[] reviewImagePixels, Stream thumbnailStream, Stream imageStream)
        {
            EnqueueWorkflow(new PhotoCaptureWorkflow(this.callback, this.cameraController, reviewImagePixels, thumbnailStream, imageStream));
        }

        #endregion

        #region Private workflow management methods

        private void EnqueueWorkflow(WorkflowBase workflow)
        {
            lock (this.workflowQueue)
            {
                // If we are unloading the camera, it's safe to clear the workflow queue
                //
                if (workflow is UnloadCameraWorkflow)
                {
                    this.workflowQueue.Clear();
                }

                this.workflowQueue.Enqueue(workflow);
            }

            MaybeProcessNextWorkflow();
        }

        private void MaybeProcessNextWorkflow()
        {
            lock (this.workflowQueue)
            {
                if ((this.currentWorkflow != null) &&
                    (this.currentWorkflow.IsComplete == false))
                {
                    return;
                }

                if (this.workflowQueue.Count > 0)
                {
                    this.currentWorkflow = this.workflowQueue.Dequeue();
                    this.currentWorkflow.Complete += OnWorkflowComplete;

                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.currentWorkflow.Begin));
                }
            }
        }

        private void OnWorkflowComplete()
        {
            this.currentWorkflow.Complete -= OnWorkflowComplete;
            MaybeProcessNextWorkflow();
        }

        #endregion
    }
}

/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8CameraFeedByteConverter
* Copyright (c) Microsoft Corporation
*
* In Windows Phone applications that use the CaptureSource class, 
* you must also use the Microsoft.Devices.Camera, Microsoft.Devices.PhotoCamera, 
* or Microsoft.Xna.Framework.Audio.Microphone class to enable audio capture 
* and accurate capability detection in the application.
* Since this sample does not need any of these classes, this unused
* class prompts the Marketplace capability detection process to add the 
* ID_CAP_MICROPHONE capability to the application capabilities list upon ingestion. 
* For more information about capability detection, see: http://go.microsoft.com/fwlink/?LinkID=204620
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
using Microsoft.Xna.Framework.Audio;

namespace CSWP8CameraFeedByteConverter
{  
    public class CapabilityPlaceholder
    {
        Microphone unusedMic = null;

        private string unusedMethod()
        {
            return unusedMic.ToString();
        }
    }
}

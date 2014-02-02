/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cs
* Project:		CSWP8GestureDetection
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to detect sharp and rotation with two points touch
* and one touch flick. By this sample, you can create your own photo viewer that 
* support picture enlargement, rotation and move/flick.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using Microsoft.Phone.Controls;
using System.Windows.Input;

namespace CSWP8GestureDetection
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
        }

        double preDistance = 0;
        double preAngle = 0;
        double preFlickX = 0;
        double preFlickY = 0;
        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            TouchPointCollection tpc = e.GetTouchPoints(touchStackPanel);
            if (tpc.Count == 2)
            {
                TouchPoint point1 = tpc[0];
                TouchPoint point2 = tpc[1];

                double X1 = point1.Position.X;
                double X2 = point2.Position.X;
                double Y1 = point1.Position.Y;
                double Y2 = point2.Position.Y;

                // Detect two fingers enlargement and shrink.
                var distance = Math.Pow((X1 - X2), 2) + Math.Pow((Y1 - Y2), 2);
                if (distance > preDistance)
                {
                    txt_SharpInfo.Text = "enlarge";
                }
                else
                {
                    txt_SharpInfo.Text = "shink";
                }
                preDistance = distance;

                // Detect rotation.
                double nowAngle = 0;
                if ((X2 - X1) == 0)
                {
                    nowAngle = 90;
                }
                else
                {
                    nowAngle = Math.Atan((Y2 - Y1) / (X2 - X1));
                }
                if (nowAngle > preAngle)
                {
                    txt_RotateInfo.Text = "clock wise rotation";
                }
                else
                {
                    txt_RotateInfo.Text = "counter clock wise rotation";
                }
                preAngle = nowAngle;
            }

            // Detect flick.
            if (tpc.Count == 1)
            {
                TouchPoint flickPoint = tpc[0];
                if (flickPoint.Action == TouchAction.Move)
                {
                    txt_FlickInfo.Text = "Move";
                    preFlickX = flickPoint.Position.X;
                    preFlickY = flickPoint.Position.Y;
                }
                else if (flickPoint.Action == TouchAction.Up)
                {
                    double nowflickX = flickPoint.Position.X;
                    double nowflickY = flickPoint.Position.Y;
                    double length = Math.Pow((nowflickX - preFlickX), 2) +
                        Math.Pow((nowflickY - preFlickY), 2);
                    if (length > 0)
                    {
                        txt_FlickInfo.Text = String.Concat("flick", length.ToString());
                    }
                }
            }
        }
    }
}
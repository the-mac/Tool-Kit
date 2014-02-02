// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.Phone.Controls;
using StarterKitPhoneComponent;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Windows.Graphics.Display;

namespace PhoneDirect3DXamlAppInterop
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Direct3DBackground m_d3dBackground = new Direct3DBackground();
        private object m_d3dContentProvider = null;

        private List<Color> m_colors;
        private int m_colorIndex;
        private int m_hitCountCube;
        private int m_hitCountSphere;
        private int m_hitCountCone;
        private int m_hitCountCylinder;
        private int m_hitCountTeapot;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            var violet = Color.FromArgb(255, 238, 130, 238);
            var indigo = Color.FromArgb(255, 75, 0, 130);
            m_colors = new List<Color> { Colors.Red, violet, indigo, Colors.Blue, Colors.Yellow, Colors.Orange };
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            SetBackgroundResolution();
        }

        private void OnPreviousColorPressed(object sender, EventArgs e)
        {
            m_colorIndex--;
            if (m_colorIndex <= 0)
                m_colorIndex = m_colors.Count - 1;

            ChangeObjectColor("Teapot_Node", m_colorIndex);
        }

        private void OnNextColorPressed(object sender, EventArgs e)
        {
            m_colorIndex++;
            if (m_colorIndex >= m_colors.Count)
                m_colorIndex = 0;

            ChangeObjectColor("Teapot_Node", m_colorIndex);
        }

        private void ChangeObjectColor(String objectName, int colorIndex)
        {
            var color = m_colors[colorIndex];
            m_d3dBackground.ChangeMaterialColor(objectName, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
        }

        private void DrawingSurfaceBackground_Tap(object sender, GestureEventArgs e)
        {
            var currentPoint = e.GetPosition(null);
            
            double tmp = 0;

            // Rotate the point to get it relative to the 
            // native rendering surface instead of the XAML rendering surface
            switch (Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    tmp = currentPoint.X;
                    currentPoint.X = currentPoint.Y;
                    currentPoint.Y = Application.Current.Host.Content.ActualWidth - tmp;
                    break;
                case PageOrientation.LandscapeRight:
                    tmp = currentPoint.X;
                    currentPoint.X = Application.Current.Host.Content.ActualHeight - currentPoint.Y;
                    currentPoint.Y = tmp;
                    break;
            }

            // Convert dips to native pixels
            currentPoint.X = DipsToPixels(currentPoint.X);
            currentPoint.Y = DipsToPixels(currentPoint.Y);

            var objName = m_d3dBackground.OnHitObject((int)currentPoint.X, (int)currentPoint.Y);
            if (objName != null)
            {
                m_d3dBackground.ToggleHitEffect(objName);

                if (objName == "Cylinder_Node")
                {
                    HitCountCylinder.Text = (++m_hitCountCylinder).ToString();
                }
                else if (objName == "Cube_Node")
                {
                    HitCountCube.Text = (++m_hitCountCube).ToString();
                }
                else if (objName == "Sphere_Node")
                {
                    HitCountSphere.Text = (++m_hitCountSphere).ToString();
                }
                else if (objName == "Cone_Node")
                {
                    HitCountCone.Text = (++m_hitCountCone).ToString();
                }
                else if (objName == "Teapot_Node")
                {
                    HitCountTeapot.Text = (++m_hitCountTeapot).ToString();
                }
            }
        }

        private void DrawingSurfaceBackground_Loaded(object sender, RoutedEventArgs e)
        {
            SetBackgroundResolution();

            // Hook-up native component to DrawingSurfaceBackgroundGrid
            if (m_d3dContentProvider == null)
                m_d3dContentProvider = m_d3dBackground.CreateContentProvider();

            DrawingSurfaceBackground.SetBackgroundContentProvider(m_d3dContentProvider);
            DrawingSurfaceBackground.SetBackgroundManipulationHandler(m_d3dBackground);
        }

        private void SetBackgroundResolution()
        {
            var screenWidth = Application.Current.Host.Content.ActualWidth;
            var screenHeight = Application.Current.Host.Content.ActualHeight;

            // Set window bounds in dips
            m_d3dBackground.WindowBounds = new Windows.Foundation.Size(
                (float)screenWidth,
                (float)screenHeight
                );

            // Set native resolution in pixels
            m_d3dBackground.NativeResolution = new Windows.Foundation.Size(
                (float)DipsToPixels(screenWidth),
                (float)DipsToPixels(screenHeight)
                );

            // Set render resolution to the full native resolution
            m_d3dBackground.RenderResolution = m_d3dBackground.NativeResolution;

            m_d3dBackground.Orientation = ConvertToNativeOrientation(this.Orientation);
        }

        private static double DipsToPixels(double dips)
        {
            return Math.Floor(dips * Application.Current.Host.Content.ScaleFactor / 100.0f + 0.5f);
        }

        private static DisplayOrientations ConvertToNativeOrientation(PageOrientation xamlOrientation)
        {
            switch (xamlOrientation)
            {
                case PageOrientation.LandscapeLeft:
                    return DisplayOrientations.LandscapeFlipped;
                case PageOrientation.LandscapeRight:
                    return DisplayOrientations.Landscape;

                case PageOrientation.PortraitDown:
                    return DisplayOrientations.PortraitFlipped;
                case PageOrientation.PortraitUp:
                    return DisplayOrientations.Portrait;

                case PageOrientation.Landscape:
                    return DisplayOrientations.Landscape;
                case PageOrientation.Portrait:
                    return DisplayOrientations.Portrait;

                case PageOrientation.None:
                default:
                    return DisplayOrientations.None;
            }
        
        }
    }
}
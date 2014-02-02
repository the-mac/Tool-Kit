//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Graphics.Interop;
using Windows.Phone.Input.Interop;
using HybridMarbleMazeComp;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HybridMarbleMaze
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Direct3DInterop m_d3dInterop = new Direct3DInterop();
        private IDrawingSurfaceBackgroundContentProvider m_backgroundContentProvider = null;
        private IDrawingSurfaceContentProvider m_contentProvider = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set window bounds in dips
            m_d3dInterop.UpdateForWindowSizeChange(
                (float)Application.Current.Host.Content.ActualWidth,
                (float)Application.Current.Host.Content.ActualHeight
                );

            // Set native resolution in pixels
            m_d3dInterop.NativeResolution = new Windows.Foundation.Size(
                (float)Math.Floor(Application.Current.Host.Content.ActualWidth  * Application.Current.Host.Content.ScaleFactor / 100.0f + 0.5f),
                (float)Math.Floor(Application.Current.Host.Content.ActualHeight * Application.Current.Host.Content.ScaleFactor / 100.0f + 0.5f)
                );

            // Set render resolution to the full native resolution
            m_d3dInterop.RenderResolution = m_d3dInterop.NativeResolution;

            m_backgroundContentProvider = m_d3dInterop.CreateBackgroundContentProvider();
            m_contentProvider = m_d3dInterop.CreateContentProvider();

            UseDrawingSurfaceBackgroundGrid(m_d3dInterop.IsDSBG());
        }

        private void OnDrawingSurfaceTypeToggle(bool isDSBG)
        {
            Dispatcher.BeginInvoke(() =>
            {
                UseDrawingSurfaceBackgroundGrid(isDSBG);
            });
        }

        public void UseDrawingSurfaceBackgroundGrid(bool useDrawingSurfaceBackgroundGrid)
        {
            if (useDrawingSurfaceBackgroundGrid)
            {
                DrawingSurface.SetContentProvider(null);
                DrawingSurface.SetManipulationHandler(null);
                DrawingSurface.IsHitTestVisible = false;

                DrawingSurfaceBackgroundGrid.SetBackgroundContentProvider(m_backgroundContentProvider);
                DrawingSurfaceBackgroundGrid.SetBackgroundManipulationHandler(m_d3dInterop);
            }
            else
            {
                DrawingSurfaceBackgroundGrid.SetBackgroundContentProvider(null);
                DrawingSurfaceBackgroundGrid.SetBackgroundManipulationHandler(null);

                DrawingSurface.SetContentProvider(m_contentProvider);
                DrawingSurface.SetManipulationHandler(m_d3dInterop);
                DrawingSurface.IsHitTestVisible = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((App)Application.Current).RootFrame.Obscured += ObscuredHandler;
            ((App)Application.Current).RootFrame.Unobscured += UnobscuredHandler;
            SystemTray.IsVisible = false;

            m_d3dInterop.DrawingSurfaceTypeToggled += OnDrawingSurfaceTypeToggle;

            m_d3dInterop.OnFocusChange(true);
            m_d3dInterop.OnResuming();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((App)Application.Current).RootFrame.Obscured -= ObscuredHandler;
            ((App)Application.Current).RootFrame.Unobscured -= UnobscuredHandler;

            m_d3dInterop.DrawingSurfaceTypeToggled -= OnDrawingSurfaceTypeToggle;

            AutoResetEvent autoEvent = new AutoResetEvent(false);

            // Block UI thread while native component asynchronously saves the
            // game state.
            Thread t = new Thread(async () =>
            {
                m_d3dInterop.OnFocusChange(false);
                await m_d3dInterop.OnSuspending();
                autoEvent.Set();
            });
            t.Start();
            autoEvent.WaitOne();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = m_d3dInterop.IsBackKeyHandled();
        }

        void ObscuredHandler(Object sender, EventArgs e)
        {
            m_d3dInterop.OnFocusChange(false);
        }

        void UnobscuredHandler(Object sender, EventArgs e)
        {
            m_d3dInterop.OnFocusChange(true);
        }
    }
}
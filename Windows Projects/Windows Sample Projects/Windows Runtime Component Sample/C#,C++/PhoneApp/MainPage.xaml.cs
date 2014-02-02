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
using System.Diagnostics;
using System.Windows;

namespace sdkWindowsRuntimeComponentWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        ComputeViewModel _computeViewModel;
        AudioViewModel _audioViewModel;
        public MainPage()
        {
            InitializeComponent();
            _computeViewModel = new ComputeViewModel();
            ComputeContentPanel.DataContext = _computeViewModel;

            _audioViewModel = new AudioViewModel();
            AudioContentPanel.DataContext = _audioViewModel;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //This sample uses the XAudio APIs and this can cause potential battery drain if the audio engine continues to run.
            // When we navigate from this page, suspend the audio, and resume it when we return (OnNavigatedTo). Note: The implementation
            // of Suspend in the Audio class we wrote checks to make sure it only tries to stop the engine if it has been started, i.e., if
            // a sound was played already.
            _audioViewModel.SuspendAudio();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
 	         base.OnNavigatedTo(e);

            // Resume the audio engine 
            _audioViewModel.ResumeAudio();
        }

        private void Panorama_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // While not entirely necessary here, since we suspend and resume audio whenever the user navigates to and from this page, 
            // we stop the audio engine when the user is not on the audio pano, since it is not in use. 
            PanoramaItem pano = MainPano.SelectedItem as PanoramaItem;
            if (pano != null)
            {
                if (pano.Header.ToString().ToLower() == "audio")
                {
                    _audioViewModel.ResumeAudio();
                }
                else
                {
                    _audioViewModel.SuspendAudio();
                }
            }
        }

        #region Basic Windows Runtime Component Interaction
        // The following code is simplified to call the Windows Runtime Component directly from this code-behind.
        // The purpose is to show you how to make a direct call to the component and deliberately does not depend 
        // on data-binding, MVVM or anything else. 
        private void ComputeSync_Click(object sender, RoutedEventArgs e)
        {
            // Clear the results
            ResultTextBox.Text = string.Empty;

            // Instantiate the ComputeComponent instance
            ComputeComponent_Phone.ComputeComponent computer = new ComputeComponent_Phone.ComputeComponent();

            // Call the synchronous ComputeResult method
            int result = computer.ComputeResult(int.Parse(Num1.Text), int.Parse(Num2.Text));

            // Display the result
            ResultTextBox.Text = result.ToString();
        }

        private async void ComputeAsync_Click(object sender, RoutedEventArgs e)
        {
            // Clear the results
            ResultTextBox.Text = string.Empty;

            // Instantiate the ComputeComponent instance
            ComputeComponent_Phone.ComputeComponent computer = new ComputeComponent_Phone.ComputeComponent();

            // Call the asynchronous ComputeResult method
            int result = await computer.ComputeResultAsync(int.Parse(Num1.Text), int.Parse(Num2.Text));

            // Display the result
            ResultTextBox.Text = result.ToString();
        }
        #endregion

    }
}

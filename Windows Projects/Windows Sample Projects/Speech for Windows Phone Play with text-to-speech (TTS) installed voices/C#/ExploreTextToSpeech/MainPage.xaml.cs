/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using ExploreTextToSpeech.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Linq;
using System.Windows;
using Windows.Phone.Speech.Synthesis;


// This sample requires the capability ID_CAP_SPEECH_RECOGNITION, which has been enabled in the WMAppManifest.xml

namespace ExploreTextToSpeech
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Declare the synthesizer object
        SpeechSynthesizer synthesizer;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MessageBox.Show(AppResources.Warn_EmulatorMode, AppResources.Warn_EmulatorModeTitle, MessageBoxButton.OK);
            }
            this.initializeSpeechObjects();
            this.initializeVoiceListBox();
        }

        // Read out the content of the TextBox when the Play button is tapped
        private async void ApplicationBarIconButton_TextToSpeech_Click(object sender, EventArgs e)
        {
            // Cancel any and all speaking from the synthesizer
            this.synthesizer.CancelAll();
            
            // Change the voice to match the selection
            this.updateSynthesizer();
            
            // Readout the text
            try
            {
                await this.synthesizer.SpeakTextAsync(this.readoutTextBox.Text.ToString());
            }
            catch
            {
                // Ignore the exception which may be generated if the synthesizer is already in the middle of saying something
            }
        }

        // Initialize any speech objects
        private void initializeSpeechObjects()
        {
            try
            {
                this.synthesizer = new SpeechSynthesizer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize the synthesizer: " + ex.Message);
            }
        }

        // Initialize the listBox with the set of installed TTS voices
        private void initializeVoiceListBox()
        {
            try
            {
                this.voiceListBox.ItemsSource =
                from voice in InstalledVoices.All
                orderby voice.Language, voice.Gender, voice.DisplayName
                select voice;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize the ListBox: " + ex.Message);
            }
        }

        // Update the synthesizer voice based on the current selection of the listBox
        private void updateSynthesizer()
        {
            try
            {
                this.synthesizer.SetVoice(voiceListBox.SelectedItem as VoiceInformation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update the synthesizer: " + ex.Message);
            }
        }
    }
}

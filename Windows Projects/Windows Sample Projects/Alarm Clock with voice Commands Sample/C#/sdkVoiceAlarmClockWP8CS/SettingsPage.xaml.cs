/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using Windows.Phone.Speech.Recognition;

namespace AlarmClockWithVoice
{
    public partial class Settings : PhoneApplicationPage
    {
        public static readonly StoredItem<bool> canAutoLock = new StoredItem<bool>("canAutoLock", true);
        public static readonly StoredItem<bool> is24Hr = new StoredItem<bool>("is24Hr", true);
        public static readonly StoredItem<bool> showSeconds = new StoredItem<bool>("showSeconds", true);
        public static readonly StoredItem<bool> enableVibration = new StoredItem<bool>("enableVibration", true);
        public static readonly StoredItem<bool> enableSpeech = new StoredItem<bool>("enableSpeech", true);
        public static readonly StoredItem<bool> alarmSet = new StoredItem<bool>("alarmSet", true);
        public static readonly StoredItem<TimeSpan> alarmTime = new StoredItem<TimeSpan>("alarmTime", TimeSpan.Zero);
        public static readonly StoredItem<string> voicePwd = new StoredItem<string>("voicePwd", "");

        private SoundEffectInstance alarmSound;
        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        private bool isCurrentlySpeaking = false;

        public Settings()
        {
            InitializeComponent();

            this.timer.Tick += Timer_Tick;

            // Initialize the alarm sound.
            this.alarmSound = SoundEffects.Alarm.CreateInstance();
            this.alarmSound.IsLooped = true;
        }

        // When the settings page is no longer the active page, store the settings variables according to the ToggleSwitches.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Stop the sound and vibration.
            this.timer.Stop();
            this.alarmSound.Stop();

            Settings.is24Hr.Value = this.hourFmtToggleSwitch.IsChecked.Value;
            Settings.showSeconds.Value = this.secFmtToggleSwitch.IsChecked.Value;
            Settings.enableVibration.Value = this.vibrationToggleSwitch.IsChecked.Value;
            Settings.enableSpeech.Value = this.speechToggleSwitch.IsChecked.Value;
        }

        // When the settings page becomes the active page, initialize the ToggleSwitch status according to the settings.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.hourFmtToggleSwitch.IsChecked = Settings.is24Hr.Value;
            this.secFmtToggleSwitch.IsChecked = Settings.showSeconds.Value;
            this.vibrationToggleSwitch.IsChecked = Settings.enableVibration.Value;
            this.speechToggleSwitch.IsChecked = Settings.enableSpeech.Value;

            this.hourFmtToggleSwitch.Content = Settings.is24Hr.Value ?
                "24 Hour Clock: ON" : "24 Hour Clock: OFF";
            this.secFmtToggleSwitch.Content = Settings.showSeconds.Value ?
                "Show Seconds: ON" : "Show Seconds: OFF";
            this.vibrationToggleSwitch.Content = Settings.enableVibration.Value ?
                "Enable Vibration: ON" : "Enable Vibration: OFF";
            this.speechToggleSwitch.Content = Settings.enableSpeech.Value ?
                "Enable Speech: ON" : "Enable Speech: OFF";
        }

        private async void VoicePwdButton_Clicked(object sender, RoutedEventArgs e)
        {
            SpeechRecognitionUIResult result = await Speech.recognizerUI.RecognizeWithUIAsync();
            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                Settings.voicePwd.Value = result.RecognitionResult.Text;
                this.isCurrentlySpeaking = true;
                await Speech.synthesizer.SpeakTextAsync("the current voice password is set to, " + result.RecognitionResult.Text);
                this.isCurrentlySpeaking = false;
            }
        }

        private async void TestVoicePwdButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Speech is not enabled.
            if (Settings.enableSpeech.Value == false)
                return;
            // The system is already speaking.
            if (this.isCurrentlySpeaking)
                return;

            this.isCurrentlySpeaking = true;
            if (Settings.voicePwd.Value == "")
                await Speech.synthesizer.SpeakTextAsync("Sorry, the voice password is not set.");
            else
                await Speech.synthesizer.SpeakTextAsync("The current voice password is " + Settings.voicePwd.Value);
            this.isCurrentlySpeaking = false;
        }

        private async void TestVolumeButton_Checked(object sender, RoutedEventArgs e)
        {
            // Vibrate only if it is enabled.
            if (Settings.enableVibration.Value)
                this.timer.Start();

            // Play the sound.
            this.alarmSound.Play();

            if (Settings.voicePwd.Value == "")
                return;

            try
            {
                // Starts speech recognition. 
                SpeechRecognitionResult recoResult = await Speech.recognizer.RecognizeAsync();
                if (recoResult.Text.Contains(Settings.voicePwd.Value))
                {
                    this.TestVolumeButton.IsChecked = false;
                }
            }
            catch (Exception)
            {
                // Do nothing.
            }
        }

        private void TestVolumeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Stop the sound and vibration.
            this.timer.Stop();
            this.alarmSound.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Vibrate for half of a second.
            VibrateController.Default.Start(TimeSpan.FromSeconds(.5));
        }

        private void ToggleSwitch_UnChecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch senderToggleSwitch = sender as ToggleSwitch;
            string toggleSwitchString = senderToggleSwitch.Content as string;
            senderToggleSwitch.Content = toggleSwitchString.Substring(0, toggleSwitchString.Length - "ON".Length) + "OFF";
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch senderToggleSwitch = sender as ToggleSwitch;
            string toggleSwitchString = senderToggleSwitch.Content as string;
            senderToggleSwitch.Content = toggleSwitchString.Substring(0, toggleSwitchString.Length - "OFF".Length) + "ON";
        }

    }
}

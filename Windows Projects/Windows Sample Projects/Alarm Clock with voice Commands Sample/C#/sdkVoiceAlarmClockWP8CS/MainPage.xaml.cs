/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace AlarmClockWithVoice
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool voiceCommandInitialized = false;
        private Clock clock;
        private RoutedEventHandler onLoadedEventHandler;

        // Constructor. Initialize the sound, speech and clock logic. Also disable idle detection.
        public MainPage()
        {
            InitializeComponent();
            
            SoundEffects.Initialize();
            Speech.Initialize();
            this.clock = new Clock(this);

            PhoneApplicationService.Current.ApplicationIdleDetectionMode =
                IdleDetectionMode.Disabled;

            this.onLoadedEventHandler = new RoutedEventHandler(OnLoaded);
            this.Loaded += onLoadedEventHandler;
        }

        // Register the voice commands. Called when the app is first launched. 
        public async void OnLoaded(object sender, EventArgs e)
        {
            if (!this.voiceCommandInitialized)
            {
                try
                {
                    Uri uri = new Uri("ms-appx:///voicecommands.xml", UriKind.Absolute);
                    await Windows.Phone.Speech.VoiceCommands.VoiceCommandService.InstallCommandSetsFromFileAsync(uri);

                    this.voiceCommandInitialized = true;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message + "\r\nVoice Commands failed to initialize.");
                }
            }
        }

        // Navigates to the alarm page.
        private void SetAlarmButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AlarmPage.xaml",
                UriKind.Relative));
        }

        // Navigates to the settings page.
        private void Settings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml",
                UriKind.Relative));
        }

        // When this page is no longer the active page, enable idle detection.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PhoneApplicationService.Current.UserIdleDetectionMode =
                IdleDetectionMode.Enabled;
        }

        // When this page becomes the active page, disable idle detection, 
        // and try to parse voice commands from the query string.
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PhoneApplicationService.Current.UserIdleDetectionMode =
                IdleDetectionMode.Disabled;
           
            try
            {
                String message = NavigationContext.QueryString["voiceCommandName"];

                // If the voice command is "alarm clock, turn alarm on"
                if (message == "alarmSetOn")
                {
                    Settings.alarmSet.Value = true;

                    if (Settings.is24Hr.Value)
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm is now set for "
                            + Settings.alarmTime.Value.Hours + " " + Settings.alarmTime.Value.Minutes);
                    }
                    else if (Settings.alarmTime.Value.Hours < 12)
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm is now set for "
                            + Settings.alarmTime.Value.Hours + " " + Settings.alarmTime.Value.Minutes + " A.M.");
                    }
                    else if (Settings.alarmTime.Value.Hours == 12)
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm is now set for "
                            + Settings.alarmTime.Value.Hours + " " + Settings.alarmTime.Value.Minutes + " P.M.");
                    }
                    else
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm is now set for "
                            + (Settings.alarmTime.Value.Hours - 12) + " " + Settings.alarmTime.Value.Minutes + " P.M.");
                    }
                }
                // Else if the voice command is "alarm clock, turn alarm off"
                else if (message == "alarmSetOff")
                {
                    Settings.alarmSet.Value = false;                    
                    await Speech.synthesizer.SpeakTextAsync("Alarm is now off");
                }
                // Else if the voice command is "alarm clock, when is my alarm set for"
                else if (message == "alarmTimeQuery")
                {
                    if (!Settings.alarmSet.Value)
                    {
                        await Speech.synthesizer.SpeakTextAsync("The alarm is not set");
                    }
                    else if (Settings.is24Hr.Value)
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm will ring at "
                            + Settings.alarmTime.Value.Hours + " " + Settings.alarmTime.Value.Minutes);
                    }
                    else if (Settings.alarmTime.Value.Hours < 12)
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm will ring at "
                            + Settings.alarmTime.Value.Hours + " " + Settings.alarmTime.Value.Minutes + " A.M.");
                    }
                    else if (Settings.alarmTime.Value.Hours == 12)
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm will ring at "
                            + Settings.alarmTime.Value.Hours + " " + Settings.alarmTime.Value.Minutes + " P.M.");
                    }
                    else
                    {
                        await Speech.synthesizer.SpeakTextAsync("Alarm will ring at "
                            + (Settings.alarmTime.Value.Hours - 12) + " " + Settings.alarmTime.Value.Minutes + " P.M.");
                    }
                }
                // Else if the voice command is "alarm clock, set alarm to {hour} {minute} {am/pm}"
                else if (message == "alarmTimeSet")
                {
                    string wholeStr = NavigationContext.QueryString["reco"];
                    string hourStr = NavigationContext.QueryString["hour"];
                    string minStr = NavigationContext.QueryString["minute"];
                    string ampmStr = NavigationContext.QueryString["ampm"];

                    int hours = Convert.ToInt32(hourStr);

                    if (ampmStr == "p.m." && hours != 12)
                        hours += 12;
                    else if (ampmStr == "a.m." && hours == 12)
                        hours = 0;

                    // Convert if the value of ["minute"] is "o clock".
                    int minutes = (minStr == "o clock") ? 0 : Convert.ToInt32(minStr);   

                    Settings.alarmTime.Value = new TimeSpan(hours, minutes, 0);

                    // Set the alarm so that the alarm will beep even if the app isn't in the foreground.
                    Clock.alarm.BeginTime = new DateTime(1, 1, 1, hours, minutes, 0);
                    
                    // The alarm must be turned on.
                    Settings.alarmSet.Value = true;

                    await Speech.synthesizer.SpeakTextAsync("The alarm is set to " + wholeStr.Substring("AlarmClock Set alarm to ".Length));
                }

                // Clear the QueryString or the page will retain the current value.
                NavigationContext.QueryString.Clear();
            }
            catch (Exception)
            {
                // This code block is reached if the app is accessed in a way other than voice commands, therefore, do nothing. 
            }
        }
    }
}

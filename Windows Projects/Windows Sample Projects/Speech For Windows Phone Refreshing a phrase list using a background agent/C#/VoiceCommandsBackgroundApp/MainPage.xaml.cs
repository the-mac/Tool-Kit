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
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Windows.Phone.Speech.VoiceCommands;


namespace VoiceCommandsBackgroundApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        String TaskName = "UpdatePhraselistTask";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check to see if the voice commands are installed.
            if (VoiceCommandService.InstalledCommandSets.ContainsKey("BackgroundAppEnus"))
            {
                tbInstructions.Text = "To test this app, tap and hold the Start button, release and then say 'Background App, Are the Tigers playing today?'";
            }
            else
            {
                tbInstructions.Text = "Install the voice commands for this app by tapping \"Install CommandSets\"";
            }

        }

        // Install the voice command sets.
        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            Uri u = new Uri("ms-appx:///BackgroundAppVCD.xml", UriKind.Absolute);

            try
            {
                await VoiceCommandService.InstallCommandSetsFromFileAsync(u);
                tbInstructions.Text = "To test this app, tap and hold the Start button, release and then say 'Background App, Are the Tigers playing today?'";
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Debug.WriteLine(err.Message + " " + err.StackTrace);
            }
        }

        // This will schedule a task to update the phraselists periodically for one day.
        // Periodic agents typically run every 30 minutes. 
        // To optimize battery life, periodic agents may be run in alignment with other background processes 
        // and therefore the execution time may drift by up to 10 minutes.
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PeriodicTask task = new PeriodicTask(TaskName);
                task.Description = "Periodic update of voice command phrase lists.";
                task.ExpirationTime = DateTime.Now.AddDays(1);

                // Remove the task and add it again if we have already added it
                if (ScheduledActionService.Find(task.Name) != null)
                {
                    ScheduledActionService.Remove(task.Name);
                }

                ScheduledActionService.Add(task);

#if DEBUG
                // Launch the background task after five seconds. 
                // LaunchForTest() should only be used for debugging purposes.
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(5));
#endif
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Debug.WriteLine(err.Message + " " + err.StackTrace);
            }
        }

        // Stop the background agent that updates the phrase lists.
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScheduledActionService.Find(TaskName) != null)
                {
                    ScheduledActionService.Remove(TaskName);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Debug.WriteLine(err.Message + " " + err.StackTrace);
            }
        }

        // Call UpdatePhraseListAsync to set phrase lists to their original state.
        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // The CommandSet key ('BackgroundAppEnus') is defined in BackgroundAppVCD.xml.
                if (VoiceCommandService.InstalledCommandSets.ContainsKey("BackgroundAppEnus"))
                {
                    VoiceCommandSet EnglishVCS = VoiceCommandService.InstalledCommandSets["BackgroundAppEnus"];
                    String[] playingTeamsList = { "Lions", "Tigers" };
                    String[] nonPlayingTeamsList = { "Bears" };
                    await EnglishVCS.UpdatePhraseListAsync("playingTeamsList", playingTeamsList);
                    await EnglishVCS.UpdatePhraseListAsync("nonPlayingTeamsList", nonPlayingTeamsList);
                    MessageBox.Show("PhraseList was reset successfully");
                }
                else
                {
                    // The key wasn't found, so the CommandSet is not installed.
                    MessageBox.Show("To reset the PhraseList, please install the CommandSets first.", "CommandSet not installed", MessageBoxButton.OK);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message + " " + err.StackTrace);
                MessageBox.Show(err.Message, "Failed to reset PhraseList", MessageBoxButton.OK);
            }
        }


    }
}

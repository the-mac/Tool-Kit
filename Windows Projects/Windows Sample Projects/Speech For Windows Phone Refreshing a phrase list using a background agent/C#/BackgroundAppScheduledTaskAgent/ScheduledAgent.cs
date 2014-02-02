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
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System.Threading.Tasks;
using Windows.Phone.Speech.VoiceCommands;

// To learn more about Background agents for Windows Phone, see
// http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202942(v=vs.105).aspx

namespace BackgroundAppScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task)
        {
            await UpdatePhraseList();

            NotifyComplete();
        }

        // Update the PhraseLists asynchronously. In this sample, this is done directly in this agent, but in a real app you 
        // would possibly make a web serivce or API call to get live data to update the PhraseLists accordingly.
        // We will also use a toast notification to let the user know when we've made an update.
        private async Task UpdatePhraseList()
        {
            Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
            toast.Title = "VoiceCommandsBackgroundApp";

            try
            {
                System.Collections.Generic.IReadOnlyDictionary<String, VoiceCommandSet> CommandSets = VoiceCommandService.InstalledCommandSets;
                VoiceCommandSet EnglishVCS = CommandSets["BackgroundAppEnus"];
                String[] playingTeamsList = { "Bears" };
                String[] nonPlayingTeamsList = { "Lions", "Tigers" };
                await EnglishVCS.UpdatePhraseListAsync("playingTeamsList", playingTeamsList);
                await EnglishVCS.UpdatePhraseListAsync("nonPlayingTeamsList", nonPlayingTeamsList);
                toast.Content = "Updated Phraselists Successfully!";
                toast.Show();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Debug.WriteLine(err.Message + " " + err.StackTrace);
                toast.Content = "Failed to update Phraselists.";
                toast.Show();
            }
        }
    }
}

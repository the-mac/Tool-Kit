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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;

namespace VoiceCommandsBackgroundApp
{
    /// <summary>
    /// Determine what command has been spoken by the user and return a result.
    /// </summary>
    public partial class ResultPage : PhoneApplicationPage
    {
        public ResultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Is this a new activation or a return from tombstone?
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {

                // Was the app launched using a voice command?
                if (NavigationContext.QueryString.ContainsKey("voiceCommandName"))
                {

                    // If so, get the name of the voice command.
                    string voiceCommandName
                      = NavigationContext.QueryString["voiceCommandName"];

                    // Used to store the name of the team that was being queried
                    string team;

                    // Used to store the phrase as recognized
                    string reco;

                    // Define app actions for each voice command name.
                    switch (voiceCommandName)
                    {
                        // Note: The strings used here are defined in the VCD (BackgroundAppVCD.xml
                        // and are CASE-SENSITIVE. If you change any of these in the VCD, make sure to 
                        // change them here.
                        case "playingTeamsCommand":
                            team = NavigationContext.QueryString["playingTeamsList"];
                            reco = NavigationContext.QueryString["reco"].ToString();
                            tbQuestion.Text = string.Format("You said:\n\"{0}\"", reco);
                            tbAnswer.Text = string.Format("Answer:\nYes, the {0} are playing.", team);
                            break;

                        case "nonPlayingTeamsCommand":
                            team = NavigationContext.QueryString["nonPlayingTeamsList"];
                            reco = NavigationContext.QueryString["reco"].ToString();
                            tbQuestion.Text = string.Format("You said:\n\"{0}\"", reco);
                            tbAnswer.Text = string.Format("Answer:\nNo, the {0} are not playing.", team);
                            break;
                        default:

                            // Should never get here. If the phone cannot interpret the command, it will display
                            // a 'Can't find that command' notification and and will not navigate to this app
                            // For example, try 'Background app Who is playing today?'
                            break;
                    }
                }
            }

        }

    }
}

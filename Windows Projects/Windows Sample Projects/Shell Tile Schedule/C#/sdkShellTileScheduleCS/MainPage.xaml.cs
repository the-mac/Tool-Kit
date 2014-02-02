/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace sdkShellTileScheduleCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        ShellTileSchedule SampleTileSchedule = new ShellTileSchedule();
        bool TileScheduleRunning = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update the tile image one time only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOneTime_Click(object sender, RoutedEventArgs e)
        {
            SampleTileSchedule.Recurrence = UpdateRecurrence.Onetime;
            SampleTileSchedule.StartTime = DateTime.Now;
            SampleTileSchedule.RemoteImageUri = new Uri(@"http://www.weather.gov/forecasts/graphical/images/conus/MaxT1_conus.png");
            SampleTileSchedule.Start();
            TileScheduleRunning = true;
        }

        /// <summary>
        /// Update the tile image for an indefinite period of time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonIndefinite_Click(object sender, RoutedEventArgs e)
        {
            SampleTileSchedule.Interval = UpdateInterval.EveryHour;
            SampleTileSchedule.Recurrence = UpdateRecurrence.Interval;
            SampleTileSchedule.RemoteImageUri = new Uri(@"http://www.weather.gov/forecasts/graphical/images/conus/MaxT1_conus.png");
            SampleTileSchedule.Start();
            TileScheduleRunning = true;
        }

        /// <summary>
        /// Update the tile image for a defined number of times
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDefined_Click(object sender, RoutedEventArgs e)
        {
            SampleTileSchedule.Interval = UpdateInterval.EveryHour;
            SampleTileSchedule.MaxUpdateCount = 50;
            SampleTileSchedule.Recurrence = UpdateRecurrence.Interval;
            SampleTileSchedule.RemoteImageUri = new Uri(@"http://www.weather.gov/forecasts/graphical/images/conus/MaxT1_conus.png");
            SampleTileSchedule.Start();
            TileScheduleRunning = true;
        }

        /// <summary>
        /// Stop the updating of the tile image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            // Attach to a shell schedule by starting it.
            if (!TileScheduleRunning)
            {
                buttonIndefinite_Click(sender, e);
            }

            SampleTileSchedule.Stop();
            TileScheduleRunning = false;
        }

    }
}

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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace AlarmClockWithVoice
{
    public partial class AlarmPage : PhoneApplicationPage
    {
        // True == clockwise, false == counterclockwise.
        private bool isClockwise = true;

        private enum ClockHandState
        {
            NotSelected,
            HourHandSelected,
            MinuteHandSelected
        }
        private ClockHandState clockHandState = ClockHandState.NotSelected;

        // This value changes when the user selects a hand and then taps the clock face.
        private double angleToGo = 0;

        // Lets the hour hand and minute-hand rotate, when the user taps a hand and then the clock face.
        private DispatcherTimer clockHandDispatcherTimer;

        public AlarmPage()
        {
            InitializeComponent();

            this.clockHandDispatcherTimer = new DispatcherTimer();
            this.clockHandDispatcherTimer.Tick += clockHandDispatcherTimer_Tick;
        }

        private string alarmTimeString
        {
            get
            {
                int hour = Settings.alarmTime.Value.Hours;
                int minute = Settings.alarmTime.Value.Minutes;
                DateTime alarmDateTime = new DateTime(1, 1, 1, hour, minute, 0);

                return "alarm time: " + alarmDateTime.ToString("h:mmtt");
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.alarmToggleSwitch.IsChecked = Settings.alarmSet.Value;
            this.alarmToggleSwitch.Content = Settings.alarmSet.Value ? "Alarm ON" : "Alarm OFF";
            this.timePicker.Value = new DateTime(1, 1, 1,
                                                 Settings.alarmTime.Value.Hours,
                                                 Settings.alarmTime.Value.Minutes,
                                                 0
                                                 );

            this.alarmTimeTextBlock.Text = alarmTimeString;

            // Update the clock image.
            // The hour hand rotates 30 degrees every hour, and rotates 0.5 degrees every minute.
            // The minute hand rotates 6 degrees every minute.
            this.hourHandRotation.Rotation = (Settings.alarmTime.Value.Hours) * 30 + (Settings.alarmTime.Value.Minutes) * 0.5 - 90;
            this.minuteHandRotation.Rotation = (Settings.alarmTime.Value.Minutes) * 6 - 90;
        }

        private void alarmToggleSwitch_ValueChange(object sender, RoutedEventArgs e)
        {
            Settings.alarmSet.Value = this.alarmToggleSwitch.IsChecked.Value;
            this.alarmToggleSwitch.Content = this.alarmToggleSwitch.IsChecked.Value ? "Alarm ON" : "Alarm OFF";
        }

        private void adjustDirectionToggleSwitch_ValueChange(object sender, RoutedEventArgs e)
        {
            // isChecked == (isClockwise == true) == clockwise
            if (this.adjustDirectionToggleSwitch.IsChecked.Value)
            {
                this.isClockwise = true;
                this.adjustDirectionToggleSwitch.Content = "Clockwise";
            }
            else
            {
                this.isClockwise = false;
                this.adjustDirectionToggleSwitch.Content = "Counterclockwise";
            }
        }

        private void timePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            // Update the alarm time.
            Settings.alarmTime.Value = new TimeSpan(this.timePicker.Value.Value.Hour, 
                                                    this.timePicker.Value.Value.Minute, 
                                                    0
                                                    );

            // Update the alarm's time settings.
            Clock.alarm.BeginTime = this.timePicker.Value.Value;

            // Update the string at the top of the screen.
            this.alarmTimeTextBlock.Text = alarmTimeString;

            // Update the clock image.
            this.hourHandRotation.Rotation = (Settings.alarmTime.Value.Hours) * 30 + (Settings.alarmTime.Value.Minutes) * 0.5 - 90;
            this.minuteHandRotation.Rotation = (Settings.alarmTime.Value.Minutes) * 6 - 90;
        }

        private void hourHand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.clockHandState == ClockHandState.MinuteHandSelected)
            {
                return;
            }

            // Do not try to merge the code below as 
            // "this.isHourHandSelected = !this.isHourHandSelected",
            // becuse the quoted code could theoretically cause a race condition.
            if (this.clockHandState == ClockHandState.HourHandSelected)
            {
                this.hourHandBlinkStoryboard.Stop();
                this.clockHandState = ClockHandState.NotSelected;
            }
            else if (this.clockHandState == ClockHandState.NotSelected)
            {
                this.hourHandBlinkStoryboard.Begin();
                this.clockHandState = ClockHandState.HourHandSelected;
            }
        }

        private void minuteHand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.clockHandState == ClockHandState.HourHandSelected)
            {
                return;
            }

            if (this.clockHandState == ClockHandState.MinuteHandSelected)
            {
                this.minuteHandBlinkStoryboard.Stop();
                this.clockHandState = ClockHandState.NotSelected;
            }
            else
            {
                this.minuteHandBlinkStoryboard.Begin();
                this.clockHandState = ClockHandState.MinuteHandSelected;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.clockHandState == ClockHandState.NotSelected)
                return;

            // Convert the click position to the ordinary de cartesan coordinate, 
            // so that we can easily calculate the tangent value, hence the angle value.
            Point position = e.GetPosition(this.clockFaceImage);
            position.X -= this.clockFaceImage.Width / 2;
            position.Y -= this.clockFaceImage.Height / 2;

            // Get the angle between the current tap position and 12 o'clock. 
            // Multiplying by 180 / Math.PI is to convert the arctan result from radius to degree.
            double tappedAngle = 180 - Math.Atan2(position.X, position.Y) * 180 / Math.PI;
            this.angleToGo = tappedAngle;

            if (this.clockHandState == ClockHandState.HourHandSelected)
            {
                // Since we now know the tapped angle, 
                // we need to deduce the current clock hand angle from the tapped angle,
                // so we can get the actual amount of angles that we need to go.
                this.UpdateAngleToGo(this.hourHandRotation.Rotation);

                // Round the angle value to a time of 30 degrees, 
                // because the min adjust value for the hour hand is 1 hour, which corresponds to 30 degrees.
                this.angleToGo = Math.Round(this.angleToGo / 30) * 30;

                this.clockHandDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            }
            else if (this.clockHandState == ClockHandState.MinuteHandSelected)
            {
                this.UpdateAngleToGo(this.minuteHandRotation.Rotation);

                // Round the angle value to to a time of 6 degrees, 
                // because the min adjust value for the minute hand is 1 minute, which corresponds to 6 degrees
                this.angleToGo = Math.Round(this.angleToGo / 6) * 6;

                this.clockHandDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            }
            this.clockHandDispatcherTimer.Start();
        }

        private void UpdateAngleToGo(double originalRotation)
        {
            if (this.isClockwise)
            {
                this.angleToGo = (this.angleToGo - Math.Round(originalRotation + 90) + 360) % 720;
            }
            else
            {
                this.angleToGo = 360 - (this.angleToGo - Math.Round(originalRotation + 90) + 360) % 720;
            }

            if (this.angleToGo < 0)
            {
                this.angleToGo += 360;
            }
            else if (this.angleToGo > 360)
            {
                this.angleToGo -= 360;
            }
        }

        private void clockHandDispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.clockHandState == ClockHandState.HourHandSelected)
            {
                if (this.isClockwise)
                {
                    this.hourHandRotation.Rotation = (this.hourHandRotation.Rotation + 30) % 720;
                }
                else
                {
                    this.hourHandRotation.Rotation = (this.hourHandRotation.Rotation + 690) % 720;
                }
                this.angleToGo -= 30;
            }
            else if (this.clockHandState == ClockHandState.MinuteHandSelected)
            {
                if (this.isClockwise)
                {
                    this.minuteHandRotation.Rotation = (this.minuteHandRotation.Rotation + 6) % 360;
                    this.hourHandRotation.Rotation = (this.hourHandRotation.Rotation + 0.5) % 720;
                }
                else
                {
                    this.minuteHandRotation.Rotation = (this.minuteHandRotation.Rotation + 354) % 360;
                    this.hourHandRotation.Rotation = (this.hourHandRotation.Rotation + 719.5) % 720;
                }
                this.angleToGo -= 6;
            }

            if (this.angleToGo <= 0)
            {
                int hours = (int)((this.hourHandRotation.Rotation + 90) % 720) / 30;
                int minutes = (int)Math.Round(((this.minuteHandRotation.Rotation + 90) % 360) / 6);
                this.timePicker.Value = new DateTime(1, 1, 1, hours, minutes, 0);

                this.minuteHandBlinkStoryboard.Stop();
                this.hourHandBlinkStoryboard.Stop();
                this.clockHandDispatcherTimer.Stop();
                this.clockHandState = ClockHandState.NotSelected;
            }
        }
    }
}

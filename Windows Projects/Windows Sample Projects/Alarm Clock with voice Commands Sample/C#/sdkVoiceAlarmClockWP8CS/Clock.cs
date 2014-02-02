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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Scheduler;
using Microsoft.Xna.Framework.Audio;


namespace AlarmClockWithVoice
{
    public class Clock
    {
        private DispatcherTimer dispatcherTimer;
        private SoundEffectInstance alarmSound;
        private TimeSpan timeToAlarm;
        private Dictionary<DayOfWeek, TextBlock> dayOfWeekTextBlock;
        private MainPage mainPage;
        public static Alarm alarm;

        public Clock(MainPage mainPage)
        {
            this.mainPage = mainPage;

            this.alarmSound = SoundEffects.Alarm.CreateInstance();
            this.alarmSound.IsLooped = true;

            this.dayOfWeekTextBlock = new Dictionary<DayOfWeek, TextBlock>();
            this.dayOfWeekTextBlock[DayOfWeek.Monday] = mainPage.monTextBlock;
            this.dayOfWeekTextBlock[DayOfWeek.Tuesday] = mainPage.tueTextBlock;
            this.dayOfWeekTextBlock[DayOfWeek.Wednesday] = mainPage.wedTextBlock;
            this.dayOfWeekTextBlock[DayOfWeek.Thursday] = mainPage.thuTextBlock;
            this.dayOfWeekTextBlock[DayOfWeek.Friday] = mainPage.friTextBlock;
            this.dayOfWeekTextBlock[DayOfWeek.Saturday] = mainPage.satTextBlock;
            this.dayOfWeekTextBlock[DayOfWeek.Sunday] = mainPage.sunTextBlock;

            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += dispatcherTimer_Tick;
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            this.dispatcherTimer.Start();

            Clock.alarm = new Alarm("alarm");
            Clock.alarm.ExpirationTime = DateTime.MaxValue;
            Clock.alarm.RecurrenceType = RecurrenceInterval.Daily;
            Clock.alarm.Sound = new Uri("Audio/alarm.wav", UriKind.Relative);
            Clock.alarm.Content = "Alarm ringing!";
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan currentTime = now.TimeOfDay;

            // Update the electronic clock.
            this.mainPage.TimeBlock.Text = GetFormattedDateTimeString(now);

            // Update the mechanical clock.
            ((CompositeTransform)mainPage.hourHand.RenderTransform).Rotation
                  = (currentTime.Hours % 12) * 30 + currentTime.Minutes * 0.5 - 90;
            ((CompositeTransform)mainPage.minuteHand.RenderTransform).Rotation
                = currentTime.Minutes * 6 - 90;
            ((CompositeTransform)mainPage.secondHand.RenderTransform).Rotation
                = currentTime.Seconds * 6 - 90;  

            foreach (TextBlock textblock in dayOfWeekTextBlock.Values)
            {
                textblock.Opacity = .3f;
            }
            this.dayOfWeekTextBlock[DateTime.Today.DayOfWeek].Opacity = 1;

            if (Settings.alarmSet.Value)
            {
                this.timeToAlarm = Settings.alarmTime.Value - DateTime.Now.TimeOfDay;
                if (this.timeToAlarm.TotalSeconds <= 0 && this.timeToAlarm.TotalSeconds >= -60)
                {
                    // Vibrate only if the vibration setting is enabled.
                    if (Settings.enableVibration.Value)
                    {
                        VibrateController.Default.Start(TimeSpan.FromSeconds(.5));
                    }
                    this.alarmSound.Play();
                }
                else if (alarmSound.State == SoundState.Playing)
                {
                    this.alarmSound.Stop();
                }
            }
            else if (alarmSound.State == SoundState.Playing)
            {
                this.alarmSound.Stop();
            }
        }

        private string GetFormattedDateTimeString(DateTime dateTime)
        {
            if (Settings.is24Hr.Value)
            {
                if (Settings.showSeconds.Value)
                {
                    return dateTime.ToString("H:mm:ss");
                }
                else
                {
                    return dateTime.ToString("H:mm");
                }

            }
            else
            {
                if (Settings.showSeconds.Value)
                {
                    return dateTime.ToString("h:mm:sstt");
                }
                else
                {
                    return dateTime.ToString("h:mmtt");
                }
            }
        }
    }
}

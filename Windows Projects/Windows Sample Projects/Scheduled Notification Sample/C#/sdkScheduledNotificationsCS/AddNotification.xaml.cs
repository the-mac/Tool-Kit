/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Windows;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Scheduler;

namespace sdkScheduledNotificationsCS
{
    public partial class AddNotification : PhoneApplicationPage
    {
        public AddNotification()
        {
            InitializeComponent();
        }

        private void ApplicationBarSaveButton_Click(object sender, EventArgs e)
        {
            // Generate a unique name for the new notification. You can choose a
            // name that is meaningful for your app, or just use a GUID.
            String name = System.Guid.NewGuid().ToString();



            // Get the begin time for the notification by combining the DatePicker
            // value and the TimePicker value.
            DateTime date = (DateTime)beginDatePicker.Value;
            DateTime time = (DateTime)beginTimePicker.Value;
            DateTime beginTime = date + time.TimeOfDay;

            // Make sure that the begin time has not already passed.
            if (beginTime < DateTime.Now)
            {
                MessageBox.Show("the begin date must be in the future.");
                return;
            }

            // Get the expiration time for the notification
            date = (DateTime)expirationDatePicker.Value;
            time = (DateTime)expirationTimePicker.Value;
            DateTime expirationTime = date + time.TimeOfDay;

            // Make sure that the expiration time is after the begin time.
            if (expirationTime < beginTime)
            {
                MessageBox.Show("expiration time must be after the begin time.");
                return;
            }

            // Determine which recurrence radio button is checked.
            RecurrenceInterval recurrence = RecurrenceInterval.None;
            if (dailyRadioButton.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Daily;
            }
            else if (weeklyRadioButton.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Weekly;
            }
            else if (monthlyRadioButton.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Monthly;
            }
            else if (endOfMonthRadioButton.IsChecked == true)
            {
                recurrence = RecurrenceInterval.EndOfMonth;
            }
            else if (yearlyRadioButton.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Yearly;
            }


            // Create a Uri for the page that will be launched if the user
            // taps on the reminder. Use query string parameters to pass 
            // content to the page that is launched.
            string param1Value = param1TextBox.Text;
            string param2Value = param2TextBox.Text;
            string queryString = "";
            if (param1Value != "" && param2Value != "")
            {
                queryString = "?param1=" + param1Value + "&param2=" + param2Value;
            }
            else if (param1Value != "" || param2Value != "")
            {
                queryString = (param1Value != null) ? "?param1=" + param1Value : "?param2=" + param2Value;
            }

            Uri navigationUri = new Uri("/ShowParams.xaml" + queryString, UriKind.Relative);

            if ((bool)reminderRadioButton.IsChecked)
            {
                Reminder reminder = new Reminder(name);
                reminder.Title = titleTextBox.Text;
                reminder.Content = contentTextBox.Text;
                reminder.BeginTime = beginTime;
                reminder.ExpirationTime = expirationTime;
                reminder.RecurrenceType = recurrence;
                reminder.NavigationUri = navigationUri;

                // Register the reminder with the system.
                ScheduledActionService.Add(reminder);
            }
            else
            {
                Alarm alarm = new Alarm(name);
                alarm.Content = contentTextBox.Text;
                alarm.Sound = new Uri("/Ringtones/Ring01.wma", UriKind.Relative);
                alarm.BeginTime = beginTime;
                alarm.ExpirationTime = expirationTime;
                alarm.RecurrenceType = recurrence;

                ScheduledActionService.Add(alarm);
            }

            // Navigate back to the main reminder list page.
            NavigationService.GoBack();

        }

        private void reminderRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            titleTextBox.IsEnabled = true;
        }

        private void alarmRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            titleTextBox.Text = "";
            titleTextBox.IsEnabled = false;
        }
    }


}

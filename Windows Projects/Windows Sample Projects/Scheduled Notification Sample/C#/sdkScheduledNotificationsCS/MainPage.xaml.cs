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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Scheduler;

namespace sdkScheduledNotificationsCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        IEnumerable<ScheduledNotification> notifications;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void ResetItemsList()
        {
            // Use GetActions to retrieve all of the scheduled actions
            // stored for this application. The type <Reminder> is specified
            // to retrieve only Reminder objects.
            //reminders = ScheduledActionService.GetActions<Reminder>();
            notifications = ScheduledActionService.GetActions<ScheduledNotification>();


            // If there are 1 or more notifications, hide the "no notifications"
            // TextBlock. IF there are zero reminders, show the TextBlock.
            //if (reminders.Count<Reminder>() > 0)
            if (notifications.Count<ScheduledNotification>() > 0)
            {
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyTextBlock.Visibility = Visibility.Visible;
            }

            // Update the ReminderListBox with the list of reminders.
            // A full MVVM implementation can automate this step.
            NotificationListBox.ItemsSource = notifications;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Reset the ReminderListBox items when the page is navigated to.
            ResetItemsList();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // The scheduled action name is stored in the Tag property
            // of the delete button for each reminder.
            string name = (string)((Button)sender).Tag;

            // Call Remove to unregister the scheduled action with the service.
            ScheduledActionService.Remove(name);

            // Reset the ReminderListBox items
            ResetItemsList();
        }

        private void ApplicationBarAddButton_Click(object sender, EventArgs e)
        {
            // Navigate to the AddReminder page when the add button is clicked.
            NavigationService.Navigate(new Uri("/AddNotification.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}

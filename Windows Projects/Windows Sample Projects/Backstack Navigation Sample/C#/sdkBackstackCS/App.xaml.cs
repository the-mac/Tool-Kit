/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace sdkBackstackCS
{
    public partial class App : Application
    {
        // UI controls on the RootFrame template.
        ListBox historyListBox;            // ListBox for listing the navigation history
        Button popLastButton;              // Button to pop the newest entry from the back stack
        Button popToSelectedButton;        // Button to pop all entries in the back stack up to the selected entry
        TextBlock currentPageTextBlock;    // TextBlock to display the current page the user is on

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // Set the template for the RootFrame to the new template we created in the Application.Resources in App.xaml
            RootFrame.Template = Resources["NewFrameTemplate"] as ControlTemplate;
            RootFrame.ApplyTemplate();

            popToSelectedButton = (VisualTreeHelper.GetChild(RootFrame, 0) as FrameworkElement).FindName("btnPopToSelected") as Button;
            popToSelectedButton.Click += new RoutedEventHandler(PopToSelectedButton_Click);

            popLastButton = (VisualTreeHelper.GetChild(RootFrame, 0) as FrameworkElement).FindName("btnPopLast") as Button;
            popLastButton.Click += new RoutedEventHandler(PopLastButton_Click);

            currentPageTextBlock = (VisualTreeHelper.GetChild(RootFrame, 0) as FrameworkElement).FindName("CurrentPage") as TextBlock;

            historyListBox = (VisualTreeHelper.GetChild(RootFrame, 0) as FrameworkElement).FindName("HistoryList") as ListBox;
            historyListBox.SelectionChanged += new SelectionChangedEventHandler(HistoryList_SelectionChanged);

            // Update the navigation history listbox whenever a navigation happens in the application
            RootFrame.Navigated += delegate { RootFrame.Dispatcher.BeginInvoke(delegate { UpdateHistory(); }); };

        }

        /// <summary>
        /// Use the BackStack property to refresh the navigation history listbox with the latest history.
        /// </summary>
        void UpdateHistory()
        {
            historyListBox.Items.Clear();
            int i = 0;

            // The BackStack property is a collection of JournalEntry objects.
            foreach (JournalEntry journalEntry in RootFrame.BackStack.Reverse())
            {
                historyListBox.Items.Insert(0, i + ": " + journalEntry.Source);
                i++;
            }

            currentPageTextBlock.Text = "[" + RootFrame.Source + "]";

            if (popLastButton != null)
            {
                popLastButton.IsEnabled = (historyListBox.Items.Count() > 0);
            }
        }

        /// <summary>
        /// Remove the last entry from the back stack
        /// </summary>
        private void PopLastButton_Click(object sender, RoutedEventArgs e)
        {
            // If RemoveBackEntry is called on an empty back stack, an InvalidOperationException is thrown.
            // Check to make sure the BackStack has entries before calling RemoveBackEntry.
            if (RootFrame.BackStack.Count() > 0)
                RootFrame.RemoveBackEntry();

            // Refresh the history list since the back stack has been modified.
            UpdateHistory();
        }

        /// <summary>
        /// Remove all entries from the back stack up to the selected item, but not including it.
        /// </summary>
        private void PopToSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure something has been selected
            if (historyListBox != null && historyListBox.SelectedIndex >= 0)
            {
                for (int i = 0; i < historyListBox.SelectedIndex; i++)
                {
                    RootFrame.RemoveBackEntry();
                }

                // Refresh the history list since the back stack has been modified.
                UpdateHistory();
            }
        }

        /// <summary>
        /// Handle SelectionChanged event for navigation history list.
        /// </summary>
        private void HistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (historyListBox != null && popToSelectedButton != null)
            {
                popToSelectedButton.IsEnabled = (historyListBox.SelectedItems.Count > 0) ? true : false;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}

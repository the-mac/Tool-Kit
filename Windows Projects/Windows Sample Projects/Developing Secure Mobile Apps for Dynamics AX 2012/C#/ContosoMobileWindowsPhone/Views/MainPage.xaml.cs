using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;

namespace ContosoMobile
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton btnSend;       
        private ApplicationBarMenuItem settingMenu;
        private Popup submitPopup = null;

        private Regex forbiddenCurrencyChars = new Regex(@"[^A-Z]", RegexOptions.IgnoreCase);       

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private MainViewModel MainViewModel
        {
            get
            {
                return (App.Current.RootVisual as PhoneApplicationFrame).DataContext as MainViewModel;
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            btnSend = ApplicationBar.Buttons[0] as ApplicationBarIconButton;           
            settingMenu = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            
            // If the user hasn't set up their configuration fully, navigate them straight over to the settings page
            if (!App.RedirectedToSettings && !MainViewModel.SettingsViewModel.IsFullyConfigured)
            {
                NavigationService.Navigate(new Uri(Settings.RelativeAddress, UriKind.Relative));
            }
            
            // This ensures we only ever do this once...otherwise the user would be unable to back out of the app
            App.RedirectedToSettings = true;
        }    

        private void APPBarSettingsMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(Settings.RelativeAddress, UriKind.Relative));
        }

        private void UploadClick(object sender, EventArgs e)
        {
            DisableUIDuringSubmission();
            MainViewModel.Submit();
        }   

        /// <summary>
        /// Executed when submission is done to show success
        /// or failure message in UI and re-enable UI elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSubmitCompleted(object sender, NotificationMessageArgs e)
        {
            // Invoke this on the UI thread to avoid a cross-thread access exception since this
            // call comes in properly on a different thread after the async calls complete
            Deployment.Current.Dispatcher.BeginInvoke( () =>
            {
                EnableUIAfterSubmission();
                MessageBox.Show(e.NotificationMessage.Message);
            } );
        }

        /// <summary>
        /// Disables all the UI buttons
        /// Loads submit progressbar control which contains a progress bar
        /// as a popup on top of the UI
        /// </summary>
        private void DisableUIDuringSubmission()
        {
            // Clears the focus from any active TextBoxes, dropping the SIP and saving the values before submission
            this.Focus();

            // Prevents the user from tapping any of the controls while submission is in progress
            this.IsHitTestVisible = false;

            if (this.submitPopup == null)
            {
                this.submitPopup = new Popup();
                this.submitPopup.Child = new SubmitProgressBar();
            }

            this.submitPopup.IsOpen = true;           
            btnSend.IsEnabled = false;
            settingMenu.IsEnabled = false;
            ApplicationBar.IsMenuEnabled = false;

            // Disable the phone auto-locking during submission time
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        /// <summary>
        /// Remove popup and enables all the buttons in the UI
        /// </summary>
        private void EnableUIAfterSubmission()
        {
            this.IsHitTestVisible = true;
            this.submitPopup.IsOpen = false;            
            btnSend.IsEnabled = true;
            settingMenu.IsEnabled = true;
            ApplicationBar.IsMenuEnabled = true;
            
            // Re-enable the ability to auto-lock
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainViewModel.SubmitCompleted += new SubmitCompletedEventHandler(OnSubmitCompleted);         
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MainViewModel.SubmitCompleted -= new SubmitCompletedEventHandler(OnSubmitCompleted);
            base.OnNavigatedFrom(e);
        }

        private void OnAmountChanged(object sender, TextChangedEventArgs e)
        {
            // When the string is empty (the user has cleared the last char), the value of 0 doesn't get pushed back to
            // the field and Amount then contains an erroneous value, so we explicitly push 0 into the field in this case.
            if (TBxAmount.Text.Length == 0)
            {
                MainViewModel.Amount = 0;
            }
        }

        private void OnCurrencyChanged(object sender, TextChangedEventArgs e)
        {
            if (forbiddenCurrencyChars.IsMatch(TBxCurrency.Text))
            {
                // Save the user's current cursor position (minus the bad char)
                int selectionStart = TBxCurrency.SelectionStart - 1;

                // Remove the bad characters
                TBxCurrency.Text = forbiddenCurrencyChars.Replace(TBxCurrency.Text, "");
                TBxCurrency.SelectionStart = selectionStart;
            }
        }

        private void OnCommentsChanged(object sender, TextChangedEventArgs e)
        {
            // Doing this explicitly so it saves as the user types (in case they hit the clear button)
            TBxComments.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
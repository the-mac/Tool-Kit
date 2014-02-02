namespace MyCompany.Visitors.Client.WindowsStore
{
    using MyCompany.Visitors.Client.WindowsStore.Model;
    using MyCompany.Visitors.Client.WindowsStore.Settings;
    using MyCompany.Visitors.Client.WindowsStore.ViewModel;
    using MyCompany.Visitors.Client.WindowsStore.Views;
    using System;
    using System.Linq;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private const string VISIT = "Visit_";
        private ViewModelLocator locator;

        /// <summary>
        /// Visitor received by NFC conecction.
        /// </summary>
        public static Visitor VisitorReceivedByNFC;

        /// <summary>
        /// Exposed, application wide, root frame to perform navigation.
        /// </summary>
        public static Frame RootFrame { get; set; }

        /// <summary>
        /// Dispatcher to execute code on UI thread.
        /// </summary>
        public static CoreDispatcher Dispatcher
        {
            get { return RootFrame.Dispatcher; }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            RootFrame = Window.Current.Content as Frame;

            // Get device culture.
            string culture = Windows.Globalization.Language.CurrentInputMethodLanguageTag;
            if (string.IsNullOrEmpty(culture))
                culture = "en-US";

            // Set the app language.
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = RootFrame;
            }

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!AppSettings.TestMode)
                {
                    if (string.IsNullOrEmpty(AppSettings.SecurityToken) || AppSettings.SecurityTokenExpirationDateTime < DateTime.Now)
                    {
                        if (!RootFrame.Navigate(typeof(AuthPage), args.Arguments))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }

                    else
                    {
                        if (args.Arguments.Contains(VISIT))
                        {
                            if (!RootFrame.Navigate(typeof(VisitDetailPage), args.Arguments))
                            {
                                throw new Exception("Failed to create visit details page");
                            }
                        }
                        else
                        {
                            if (!RootFrame.Navigate(typeof(MainPage), args.Arguments))
                            {
                                throw new Exception("Failed to create initial page");
                            }   
                        }
                    }
                }
                else
                {
                    if (args.Arguments.Contains(VISIT))
                    {
                        if (!RootFrame.Navigate(typeof (VisitDetailPage), args.Arguments))
                        {
                            throw new Exception("Failed to create visit details page");
                        }
                    }
                    else
                    {
                        if (!RootFrame.Navigate(typeof(MainPage), args.Arguments))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                }
            }
            else if (args.Arguments.Contains(VISIT))
            {
                RootFrame.Navigate(typeof(VisitDetailPage), args.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            SettingsPanelHelper.Instance.CreateSettingsOptions();

            locator = (ViewModelLocator)App.Current.Resources["Locator"];
            locator.NfcService.VisitorReceived += NfcService_VisitorReceived;
            locator.NfcService.WaitForDataAsync();
        }

        /// <summary>
        /// A visitor has been received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NfcService_VisitorReceived(object sender, VisitorEventArgs e)
        {
            var visitor = await locator.MyCompanyClientService.VisitorService.GetVisitors(e.Visitor.Email, PictureType.Big, 1, 0);
            if (visitor.Any())
            {
                //The visitor exist, get the next visit.
                var visits = await locator.MyCompanyClientService.VisitService.GetVisitsFromDate(e.Visitor.Email, PictureType.Big, 1, 0, DateTime.Today.ToUniversalTime());
                if (visits.Any())
                {
                    //Navigate to exiting visit.
                    RootFrame.Navigate(typeof(VisitDetailPage), visits.First().VisitId);
                }
                else
                {
                    //Navigate to new visit.
                    RootFrame.Navigate(typeof(NewVisitPage), visitor.First().VisitorId);
                }
            }
            else
            { 
                //Navigate to new Visitor
                VisitorReceivedByNFC = e.Visitor;
                RootFrame.Navigate(typeof(NewVisitorPage), true);
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

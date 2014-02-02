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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sdkAzureVoiceNotesWP8CS.Resources;

using Microsoft.WindowsAzure.MobileServices;
using Windows.Phone.Speech.Recognition;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;


namespace sdkAzureVoiceNotesWP8CS
{
    /*Note: Make sure to update the MobileServiceClient object in App.xaml.cs 
     *    with your specific mobile service URL and application key. For details
     *    about generating a mobile service URL and application, see the readme
     *    provided with this sample.
    */


    //The VoiceNote class maps the columns of the Azure database table
    //for the Voice Note application to an object type. Note that the DataMemberAttribute allows
    //the object to have property names that differ from the column names in the database.
    public class VoiceNote
    {
        [DataMember(Name = "id")]
        public int Identity { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime? TimeUTC { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "userId")]
        public string UserID { get; set; }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        //Variable for the authenticated mobile service user ID 
        private MobileServiceUser user;

        //Interface to the mobile service table 
        private IMobileServiceTable<VoiceNote> noteTable = App.MobileService.GetTable<VoiceNote>();

        //Collection for holding table query results and binding to UI        
        public ObservableCollection<VoiceNote> notes { get; set; }

        //Boolean to track first load of page. Used for initiating authentication logic
        private bool isNewPageInstance;

        //Declaration of SpeechRecognizer object
        private SpeechRecognizerUI recoWithUI;



        public MainPage()
        {
            InitializeComponent();
            isNewPageInstance = true;
        }

        // Asynchronous method for user authentication using a Microsoft Account
        // Note: Application must be registered for Microsoft Account authentication
        //       see readme file for additional details.
        private async Task AuthenticateAsync()
        {
            //Verify network connectivity is available before attempting authentication
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection unavailable. Please close app, verify your network connection, and try again.");
                return;
            }

            bool authenticated = true;

            //Attempt login using a Microsoft Account, watching for an exception. An exception
            //occurs if the user presses the hardware back button without completing login. Note
            //that while this application uses Microsoft Account, other authentication accounts 
            //(Facebook, Twitter, and Google) can be used.
            try
            {
                user = await App.MobileService
                    .LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
            }
            catch (InvalidOperationException)
            {
                authenticated = false;
            }

            if(!authenticated)
                MessageBox.Show("Error with authentication. Please tap the login button on the app bar to try again.");

        }

        // Event handler for refresh button press, used to manually initiate
        // an asynchronous update of the voice note list against the applications
        // Azure database.
        private void refBtn_Click(object sender, EventArgs e)
        {
            RefreshVoiceNotesAsync();
        }

        // Updates the entries of the list view by requerying the Azure database table. The query 
        // returns all valid results for a user (by authenticated ID). For additional details about
        // configuring the database read script, see the readme file provided with this sample.
        private async void RefreshVoiceNotesAsync()
        {
            //Verify network connectivity is available before attempting authentication
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection unavailable. Please close app, verify your network connection, and try again.");
                return;
            }
           
            bool retrieved = true;

            //Attempt to retrieve the latest Azure database table information, updating
            //the UI if results are retrieved.
            List<VoiceNote> result = new List<VoiceNote>();
            try
            {
                result = await noteTable.ToListAsync();
            }
            catch (MobileServiceInvalidOperationException)
            {
                retrieved = false;
            }

            if (retrieved)
            {
                notes = new ObservableCollection<VoiceNote>(result);
                NoteList.ItemsSource = notes;
            }
            else
                MessageBox.Show("Error retrieving data. Retry login.");          
        }

        // Create and configure the SpeechRecognizerUI object.
        private void ConfigureRecognizer()
        {
            recoWithUI = new SpeechRecognizerUI();
            recoWithUI.Settings.ListenText = "Speak your voice reminder.";
            recoWithUI.Settings.ReadoutEnabled = true;
            recoWithUI.Settings.ShowConfirmation = true;
        }

        // Initiate the capture of a voice note and store it to the 
        // Azure database if the user is satisfied
        private async void speechBtn_Click(object sender, EventArgs e)
        {
            // Begin recognition using the default grammar and store the result.
            SpeechRecognitionUIResult recoResult = await recoWithUI.RecognizeWithUIAsync();

            // Check that a result was obtained
            if (recoResult.RecognitionResult != null)
            {
                // Determine if the user wants to save the note.
                var result = MessageBox.Show(string.Format("Heard you say \"{0}\" Save?", recoResult.RecognitionResult.Text), "Confirmation", MessageBoxButton.OKCancel);

                // Save the result to the Azure Mobile Service DB if the user is satisfied.
                if (result == MessageBoxResult.OK)
                {
                    var note = new VoiceNote { Text = recoResult.RecognitionResult.Text };
                    AddVoiceNoteAsync(note);
                }
            }
        }

        // Add a voice note to the Azure database and the local 
        // CollectionView for display if successful
        private async void AddVoiceNoteAsync(VoiceNote note)
        {
            //Verify network connectivity is available before attempting to add note to database
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection unavailable. Please close app, verify your network connection, and try again.");
                return;
            }
            
            bool added = true;
            try
            {
                await noteTable.InsertAsync(note);
            }
            catch (MobileServiceInvalidOperationException)
            {
                added = false;
            }

            if (added)
                notes.Add(note);
            else
                MessageBox.Show("Error adding note. The selected note was not added to the server. Verify that you are logged in and that a network connection is available.");    
        }   

        // Delete the user selection of voice notes from the Azure database
        private void delBtn_Click(object sender, EventArgs e)
        {
            //Prompt user for confirmation before deletion.
            var result = MessageBox.Show(string.Format("Delete {0} voice note(s)", NoteList.SelectedItems.Count), "Delete confirmation", MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
                return;

            //Delete the currently selected voice notes.
            foreach (VoiceNote vn in NoteList.SelectedItems)
            {
                DeleteNoteAsync(vn);
            }
        }

        // Removes a specific VoiceNote from the Azure database. When the Azure Mobile Service 
        // responds, the item is removed from the local list
        private async void DeleteNoteAsync(VoiceNote note)
        {
            //Verify network connectivity is available before attempting to add note to database
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection unavailable. Please close app, verify your network connection, and try again.");
                return;
            }

            bool deleted = true;
            try
            {
                await noteTable.DeleteAsync(note);
            }
            catch (MobileServiceInvalidOperationException)
            {
                deleted = false;
            }

            if (deleted)
                notes.Remove(note);
            else
            {
                MessageBox.Show("Error with deletion. The selected note(s) have not been removed from the server.");
            }
            
        }

        // Manual initiation of authentication, with voice note update on success
        private async void loginBtn_Click(object sender, EventArgs e)
        {
            if (user != null)
            {
                MessageBox.Show("Already logged in. Please exit the app and relaunch to log in as different user.");
                return;
            }

            await AuthenticateAsync();
            if (user != null)
                RefreshVoiceNotesAsync();              
        }

        // Configure application bar 
        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBarIconButton loginButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/check.png", UriKind.Relative));
            loginButton.Text = "Login";
            ApplicationBar.Buttons.Add(loginButton);
            loginButton.Click += new EventHandler(loginBtn_Click);
            ApplicationBarIconButton speechButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/microphone.png", UriKind.Relative));
            speechButton.Text = "Speak";
            ApplicationBar.Buttons.Add(speechButton);
            speechButton.Click += new EventHandler(speechBtn_Click);
            ApplicationBarIconButton deleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            deleteButton.Text = "Delete";
            ApplicationBar.Buttons.Add(deleteButton);
            deleteButton.Click += new EventHandler(delBtn_Click);
            ApplicationBarIconButton refreshButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/refresh.png", UriKind.Relative));
            refreshButton.Text = "Refresh";
            ApplicationBar.Buttons.Add(refreshButton);
            refreshButton.Click += new EventHandler(refBtn_Click);

        }

        // Configure app bar and voice recognition, as well as initiate authentication and voice note refresh
        // the first time page is navigated to.
        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isNewPageInstance)
            {
                isNewPageInstance = false;
                BuildApplicationBar();
                ConfigureRecognizer();
                await AuthenticateAsync();
                if(user != null)
                   RefreshVoiceNotesAsync();
            }
        }

        // Ensure logout from Azure Mobile Services when user leaves application
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (user != null)
            {
                App.MobileService.Logout();
            }            
        }
    }
}

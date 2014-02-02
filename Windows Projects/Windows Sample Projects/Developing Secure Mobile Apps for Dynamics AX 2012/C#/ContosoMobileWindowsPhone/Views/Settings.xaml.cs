using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ContosoMobile
{
    public partial class Settings : PhoneApplicationPage
    {
        private string userName;
        private string password;
        private string azureNamespace;
        private string adfsUrl;

        public const string RelativeAddress = "/ContosoMobile;component/Views/Settings.xaml";

        public Settings()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {          
            // Save these values for later
            userName = TBxUsername.Text;
            password = TBxPassword.Password;            
            azureNamespace = TBxAppFabricNamespace.Text;
            adfsUrl = TBxStsEndpoint.Text;

            // On first time load, place the focus on the username
            if (string.IsNullOrWhiteSpace(TBxUsername.Text))
            {
                TBxUsername.Focus();
            }
        }

        private void SaveClick(object sender, EventArgs e)
        {
            // Update all the values (only seems to be needed if the user does not have focus in a TextBox)
            TBxUsername.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            TBxPassword.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();           
            TBxAppFabricNamespace.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            TBxStsEndpoint.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            
            NavigationService.GoBack();
        }

        private void CancelClick(object sender, EventArgs e)
        {
            // We have to do this because when the user has focus in a TextBox, for some reason it saves the values...
            // So we have to revert them back to their originals just for that case.  When the user hits the back button,
            // everything works just fine regardless if the focus is in a TextBox or not!
            TBxUsername.Text = userName;
            TBxPassword.Password = password;            
            TBxAppFabricNamespace.Text = azureNamespace;
            TBxStsEndpoint.Text = adfsUrl;

            NavigationService.GoBack();
        }       
    }
}
using ContosoMobile.Authentication;
using ContosoMobile.ExpenseServiceReference;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows;
using System.Xml;

namespace ContosoMobile
{
    /// <summary>
    /// The view model for MainPage
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region private fields

        private SettingsViewModel settingsViewModel;

        // Expense fields        
        private string currency = string.Empty;
        private DateTime expenseDate = DateTime.Now;
        private decimal amount = 0;
        private string comment = string.Empty;      

        //Authentication Provider object
        private AuthenticationProvider authenticator;
        
        #endregion

        #region properties

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                if (settingsViewModel == null)
                {
                    settingsViewModel = new SettingsViewModel();
                }

                return settingsViewModel;
            }
        }        
       
        public string Currency
        {
            get
            {
                return currency;
            }
            set
            {
                if (currency != value)
                {
                    currency = value;
                    RaisePropertyChanged("Currency");
                }
            }
        }

        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                if (comment != value)
                {
                    comment = value;
                    RaisePropertyChanged("Comment");
                }
            }
        }

        public DateTime ExpenseDate
        {
            get
            {
                return expenseDate;
            }
            set
            {
                if (expenseDate != value)
                {
                    // Do not allow dates in the future
                    expenseDate = value > DateTime.Now.Date ? DateTime.Now.Date : value;
                    RaisePropertyChanged("ExpenseDate");
                }
            }
        }

        /// <summary>
        /// This read-only property returns whether the form is empty.
        /// </summary>
        /// <remarks>
        /// Changing the currency & date fields are not considered entering any information.
        /// </remarks>
        private bool IsTransactionBlank
        {
            get
            {
                return this.Amount == 0
                    && string.IsNullOrWhiteSpace(this.Comment);                   
            }
        }
        #endregion

        #region public methods

        /// <summary>
        /// Authenticate the user, fetch the required tokens and submit the expense
        /// </summary>
        public void Submit()
        {
            if (CanSubmit())
            {               
                authenticator = new AuthenticationProvider(settingsViewModel.UserName, settingsViewModel.Password, settingsViewModel.AzureNamespace, settingsViewModel.StsEndpoint);
                authenticator.AuthenticationCompleted += authenticator_AuthenticationCompleted;
                //Start the async calls to fetch the SAML and SWT tokens
                authenticator.IssueToken();
            }
        }

        #endregion

        #region events & event methods

        /// <summary>
        /// Call this method when the submission has failed and you would like to raise a generic error to the user.
        /// </summary>
        /// <param name="e">The exception that was raised to be printed out in debug mode; optional</param>
        private void RaiseSubmitCompleted(Exception e)
        {
            string genericError = "The submission failed. Please try again later.";

            RaiseSubmitCompleted(false, genericError, e);
        }

        /// <summary>
        /// This method should be called when the expense submission is successful or when the failure
        /// reason is known and the caller has a specific message they would like to surface to the UI.
        /// </summary>
        /// <param name="success">true if the submission has completed successfully; otherwise false.</param>
        /// <param name="message">A string describing the event to be displayed in the UI.</param>
        /// <param name="e">The exception that was raised to be displayed in debug mode; optional.</param>
        private void RaiseSubmitCompleted(bool success, string message, Exception e)
        {
            NotificationMessage notificationMessage;

#if DEBUG
            if (!success)
            {
                message += "\n\nDEBUG info: ";
                message += (e == null ? "Submit failed; no exception found." : e.ToString());
            }
#endif
            notificationMessage = new NotificationMessage(message, success);

            // Explicitly declaring for thread safety
            SubmitCompletedEventHandler handler = this.SubmitCompleted;

            // Send message to the view to display to the user
            if (handler != null)
            {
                handler(this, new NotificationMessageArgs(notificationMessage));
            }
        }

        /// <summary>
        /// Called by a property when the value is changed in order to raise a PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            // Explicitly declaring for thread safety
            PropertyChangedEventHandler handler = this.PropertyChanged;
            
            if (handler != null)
            {
                // Since a two-way binding would update the UI thread,
                // this needs to be dispatched correctly at this point
                Deployment.Current.Dispatcher.BeginInvoke( () =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                } );
            }
        }

        /// <summary>
        /// Subscribe to this event to be notified of expense submission completion messages, both succeeded & failed.
        /// </summary>
        public event SubmitCompletedEventHandler SubmitCompleted;

        /// <summary>
        /// Subscribe to this event to be notified of changes to property values.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private methods

        private void authenticator_AuthenticationCompleted(object sender, AuthenticationMessageArgs e)
        {
            if (e.AuthenticationMessage.IsSuccess)
            {
                //Authentication was successful. Now submit the expense through service bus, 
                //using the SWT and SAML token values exposed by the AuthenticationProvider
                CreateExpense();
            }
            else
            {
                RaiseSubmitCompleted(new Exception(e.AuthenticationMessage.Message));
            }
        }  

        private void CreateExpense()
        {
            ExpenseServiceReference.ExpenseServiceContractClient expenseService = new ExpenseServiceReference.ExpenseServiceContractClient();
            expenseService.Endpoint.Address = new EndpointAddress(new Uri(SettingsViewModel.ServiceEndpoint));
            expenseService.CreateExpenseCompleted += new EventHandler<CreateExpenseCompletedEventArgs>(OnCreateExpenseCompleted);

            // Get the channel context scope so we can add message headers to it
            using (new OperationContextScope(expenseService.InnerChannel))
            {
                // Grab the raw SAML token and stuff it in a byte array               
                byte[] binToken = Encoding.UTF8.GetBytes(authenticator.SamlAssertionToken);                
                
                // Add the SAML token to a custom SOAP header               
                var customSecurityHeader = MessageHeader.CreateHeader("PassthroughBinarySecurityToken", "", Convert.ToBase64String(binToken));
                OperationContext.Current.OutgoingMessageHeaders.Add(customSecurityHeader);

                // Add the SWT token to a SOAP header as SB is expecting
                OperationContext.Current.OutgoingMessageHeaders.Add(new AcsHeader(authenticator.SwtAcsToken));

                // Call the actual service to create our expense.                
                expenseService.CreateExpenseAsync(ExpenseDate, Amount, Amount == 0 ? null : Currency, Comment);
            }
        }

        private void OnCreateExpenseCompleted(object sender, CreateExpenseCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                RaiseSubmitCompleted(true, "Your expense has been submitted successfully.", null);
            }
            else
            {
                if (e.Error is FaultException && e.Error.Message == "No service is hosted at the specified address.")
                {
                    RaiseSubmitCompleted(false, "Service not found. Please check the service connection name in settings and try again.", e.Error);
                    return;
                }           
                RaiseSubmitCompleted(e.Error);
            }
        }        

        /// <summary>
        /// Check to see if all the required information is present in the Settings (user credentials, azure namespace, adfs endpoint Url)
        /// An amount should be entered too
        /// </summary>
        /// <returns></returns>
        private bool CanSubmit()
        {
            string errorMessage;

            if (string.IsNullOrWhiteSpace(SettingsViewModel.UserName) || string.IsNullOrWhiteSpace(SettingsViewModel.Password))
            {
                errorMessage = "Please specify your username and password in the settings.";
            }
            else if (string.IsNullOrWhiteSpace(SettingsViewModel.AzureNamespace))
            {
                errorMessage = "Please specify the service connection name in the settings.";
            }
            else if (string.IsNullOrWhiteSpace(SettingsViewModel.StsEndpoint))
            {
                errorMessage = "Please specify the authentication server URL in the settings.";
            }
            else if (this.IsTransactionBlank)
            {
                errorMessage = "Please enter data to submit.";
            }
            else
            {
                return true;
            }

            RaiseSubmitCompleted(false, errorMessage, null);
            return false;
        }

        #endregion

        #region nested types

        /// <summary>
        /// This is a very special (SOAP header) format that Service Bus requires a token from ACS to exist in the message.
        /// This header will be stripped out by SB and thus will not exist when the message gets to the service.
        /// </summary>
        /// <remarks>
        /// Leave all these hard-coded values alone, SB is expecting a very particular format for this header.
        /// </remarks>
        private class AcsHeader : MessageHeader
        {
            private string token;

            public AcsHeader(string token)
            {
                this.token = token;
            }

            public override string Name
            {
                get { return "RelayAccessToken"; }
            }
            
            public override string Namespace
            {
                get { return "http://schemas.microsoft.com/netservices/2009/05/servicebus/connect"; }
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteStartElement("BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                writer.WriteAttributeString("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd",
                                            string.Format("uuid:{0}", Guid.NewGuid().ToString("D")));
                writer.WriteAttributeString("ValueType", "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0");
                writer.WriteAttributeString("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
                
                byte[] binToken = Encoding.UTF8.GetBytes(token);
                writer.WriteBase64(binToken, 0, binToken.Length);
                
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
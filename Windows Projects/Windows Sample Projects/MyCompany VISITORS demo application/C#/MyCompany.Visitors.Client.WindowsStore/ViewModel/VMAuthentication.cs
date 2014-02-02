namespace MyCompany.Visitors.Client.WindowsStore.ViewModel
{
    using GalaSoft.MvvmLight;
    using Microsoft.Preview.WindowsAzure.ActiveDirectory.Authentication;
    using MyCompany.Visitors.Client.WindowsStore.Services.Navigation;
    using MyCompany.Visitors.Client.WindowsStore.Settings;
    using System;

    /// <summary>
    /// Authentication page viewmodel.
    /// </summary>
    public class VMAuthentication : ViewModelBase
    {
        private readonly INavigationService navService;
        private readonly IMyCompanyClient myCompanyClient;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VMAuthentication(INavigationService navService, IMyCompanyClient myCompanyClient)
        {
            this.navService = navService;
            this.myCompanyClient = myCompanyClient;
        }

        /// <summary>
        /// Authenticate an user and navigate to mainpage if success.
        /// </summary>
        public async void AuthenticateUser()
        {
            AuthenticationContext.TokenCache = null;
            var authenticationContext = new AuthenticationContext(AppSettings.AuthenticationUri);

            AuthenticationResult authResult = await authenticationContext.AcquireTokenAsync(
                                                                AppSettings.ApiUri.ToString(),
                                                                AppSettings.ClientId,
                                                                AppSettings.ReplyUri,
                                                                string.Empty, string.Empty);

            if (authResult.ExpiresOn < DateTime.UtcNow)
            {
                authResult = await authenticationContext.AcquireTokenByRefreshTokenAsync(authResult.RefreshToken, AppSettings.ClientId);
            }


            if (authResult.Status == AuthenticationStatus.Succeeded)
            {
                myCompanyClient.RefreshToken(authResult.AccessToken);
                AppSettings.SecurityToken = String.Format("Bearer {0}", authResult.AccessToken);
                AppSettings.SecurityTokenExpirationDateTime = authResult.ExpiresOn;
                this.navService.NavigateToMainPage();
            }
        }
    }
}

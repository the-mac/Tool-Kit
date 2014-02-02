using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using WPPushNotification.ServerSideWeatherSimulator.Service;
using System.ServiceModel.Description;

namespace WPPushNotification.ServerSideWeatherSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ServiceHost host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                //TODO - remove remark after creating registration service
                host = new ServiceHost(typeof(RegistrationService));
                host.Open();
            }
            catch (TimeoutException timeoutException)
            {
                MessageBox.Show(String.Format("The service operation timed out. {0}", timeoutException.Message));
            }
            catch (CommunicationException communicationException)
            {
                MessageBox.Show(String.Format("Could not start service host. {0}", communicationException.Message));
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (host != null)
            {
                try
                {
                    host.Close();
                }
                catch (TimeoutException)
                {
                    host.Abort();
                }
                catch (CommunicationException)
                {
                    host.Abort();
                }
            }
            base.OnExit(e);
        }
    }
}

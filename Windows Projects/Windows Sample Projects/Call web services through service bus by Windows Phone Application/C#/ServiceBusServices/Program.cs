/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      ServiceBusServices
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to expose an on-premise REST service to Internet
via Service Bus, then you can access this service by Windows Phone application.
The service includes normal string, generics and image methods.

We use ServiceHost class to expose the service to the Microsoft Windows Azure
Service Bus, here you need input your Service Bus issuer and key before running
the Console application.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using Microsoft.ServiceBus;
using System.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel;

namespace ServiceBusServices
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceNamespace = ConfigurationManager.AppSettings["serviceNamespace"];
            Uri address = ServiceBusEnvironment.CreateServiceUri("http", serviceNamespace, "");

            // Create WebHttpRelayBinding instance.
            WebHttpRelayBinding binding = new WebHttpRelayBinding(EndToEndWebHttpSecurityMode.None, RelayClientAuthenticationType.None);
            
            // Create ServiceHost with endpoint.
            var host = new ServiceHost(typeof(WindowsPhoneService), address);
            host.AddServiceEndpoint(typeof(IWindowsPhoneService), binding, address);
            var behavior = host.Description.Endpoints[0].Behaviors;

            // Add ServiceBus key.
            behavior.Add(new TransportClientEndpointBehavior(TokenProvider.CreateSharedSecretTokenProvider(ConfigurationManager.AppSettings["issuer"], ConfigurationManager.AppSettings["key"])));
            behavior.Add(new WebHttpBehavior());
            host.Open();

            Console.WriteLine("Service listening at: " + host.Description.Endpoints[0].Address);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            host.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WPPushNotification.ServerSideWeatherSimulator.Service
{
    [ServiceContract]
    public interface IRegistrationService
    {
        [OperationContract, WebGet]
        void Register(string uri);

        [OperationContract, WebGet]
        void Unregister(string uri);

        /// <summary>
        /// Allow a client to actively request the latest data.
        /// </summary>
        /// <param name="locationName">The name of the location for which data is requested.</param>
        /// <param name="uri">The uri to the client requesting the data.</param>
        [OperationContract, WebGet]
        void RequestData(string locationName, string uri);
    }
}

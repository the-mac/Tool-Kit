using System;
using System.Net;
using WindowsPhone.Recipes.Push.Messasges;

namespace WPPushNotification.ServerSideWeatherSimulator
{
    #region NotificationType Enum
    public enum NotificationType
    {
        Token = 1,
        Toast = 2,
        Raw = 3
    }
    #endregion

    /// <summary>
    /// Converts push notification result to log message
    /// </summary>
    public class PushNotificationsLogMsg
    {
        #region Ctor

        public PushNotificationsLogMsg(NotificationType notificationType, MessageSendResult messageSendResult)
        {
            this.Timestamp = messageSendResult.Timestamp;
            this.MessageId = messageSendResult.AssociatedMessage.Id.ToString();
            this.ChannelUri = messageSendResult.ChannelUri.ToString();
            this.NotificationType = notificationType;
            this.StatusCode = messageSendResult.StatusCode;
            this.NotificationStatus = messageSendResult.NotificationStatus.ToString();
            this.DeviceConnectionStatus = messageSendResult.DeviceConnectionStatus.ToString();
            this.SubscriptionStatus = messageSendResult.DeviceConnectionStatus.ToString();
        }

        #endregion

        #region Properties

        public DateTimeOffset Timestamp { get; private set; }
        public string MessageId { get; private set; }
        public string ChannelUri { get; private set; }
        public NotificationType NotificationType { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string NotificationStatus { get; private set; }
        public string DeviceConnectionStatus { get; private set; }
        public string SubscriptionStatus { get; private set; }

        #endregion
    }
}

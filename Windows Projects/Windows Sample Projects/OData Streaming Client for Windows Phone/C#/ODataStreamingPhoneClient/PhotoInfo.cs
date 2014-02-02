using System;
using System.Data.Services.Client;

namespace ODataStreamingPhoneClient.Model
{
    public partial class PhotoInfo
    {
        // Returns the media resource URI for binding.
        public Uri StreamUri
        {
            get
            {
                return App.ViewModel.GetReadStreamUri(this);             
            }
        }
    }

    public partial class PhotoInfo
    {
        // Provides a way to report an update to the stream URI.
        public void ReportStreamUriUpdated()
        {
            this.OnPropertyChanged("StreamUri");
        }
    }
}

// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using ODataWinPhoneQuickstart;

namespace ODataWinPhoneQuickstart.DataServiceContext.Netflix
{
    // Extend the Title class to bind to the media resource URI.
    public partial class Title 
    {
        private BitmapImage _image;

        // Returns the media resource URI for binding.
        public BitmapImage DefaultImage
        {
            get
            {
                if (_image == null)
                {
                    // Get the URI for the media resource stream.
                    return App.ViewModel.GetImage(this);
                }
                else
                {
                    return _image;
                }
            }
            set
            {
                _image = value;
                OnPropertyChanged("DefaultImage");
            }
        }
    }
}

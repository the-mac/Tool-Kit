// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using ODataWinPhoneQuickstart;

namespace ODataWinPhoneQuickstart.Netflix
{
    // Extend the Title class to bind to the media resource URI.
    public partial class Title
    {
        // Returns the media resource URI for binding.
        public Uri StreamUri
        {
            get
            {
                // Get the URI for the media resource stream.
                return App.ViewModel.GetReadStreamUri(this);
            }
        }
    }
}

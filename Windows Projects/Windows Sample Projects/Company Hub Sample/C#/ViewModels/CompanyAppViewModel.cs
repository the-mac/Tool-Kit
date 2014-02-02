/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Phone.Management.Deployment;

namespace CompanyHubExample
{
    public enum Status
    {
        Installed,
        NotInstalled,
        Installing,
        InstallFailed,
        Canceled
    }

    public class CompanyAppViewModel : INotifyPropertyChanged
    {
        private Uri image;
        private Uri xapPath;
        private string description;
        private string title;
        private Status status;
        private Guid productId;
        private uint progressValue;
        private Windows.Foundation.IAsyncOperationWithProgress<PackageInstallResult, uint> result;

        public CompanyAppViewModel()
        {
            // Keep a default constructor so that the design view works.
        }

        public CompanyAppViewModel(string title, string description, string imagePath, Uri xapPath, Status status, Guid productId)
        {
            // Get rid of any HTML-encoded characters (i.e. convert &lt; to '<').
            this.Title = System.Net.HttpUtility.HtmlDecode(title);

            this.Description = description;
            this.Image = new Uri(imagePath);
            this.XapPath = xapPath;
            this.Status = status;
            this.ProductId = productId;
        }

        public Windows.Foundation.IAsyncOperationWithProgress<PackageInstallResult, uint> Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
                NotifyPropertyChanged();
            }
        }

        public Guid ProductId
        {
            get
            {
                return this.productId;
            }
            set
            {
                if (value != this.productId)
                {
                    this.productId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Uri Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (value != this.image)
                {
                    this.image = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value != this.title)
                {
                    this.title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Status Status
        {
            get
            {
                return this.status;
            }
            set
            {
                if (value != this.status)
                {
                    this.status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Uri XapPath
        {
            get
            {
                return this.xapPath;
            }
            set
            {
                if (value != this.xapPath)
                {
                    this.xapPath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public uint ProgressValue
        {
            get
            {
                return this.progressValue;
            }
            set
            {
                if (value != this.progressValue)
                {
                    this.progressValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

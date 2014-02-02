/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Wallet;

namespace sdkWalletMembershipDealsWP8CS
{
    /// <summary>
    /// A wrapper class for a wallet deal implementing INotify to allow for data context binding
    /// </summary>
    public class Coupon : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor for a coupon which encapsulates a deal
        /// </summary>
        /// <param name="id">ID associated with the coupon</param>
        /// <param name="title">title of the coupon</param>
        public Coupon(string id, string title)
        {
            this.id = id;
            this.title = title;
        }

        // private fields accessed by properties below, strings initialized with empty are optional
        private string id;
        private string title;
        private string description = string.Empty;
        private string terms = string.Empty;
        private string code = string.Empty;
        private string walletStatus;
        private DateTime expirationDate;        
        private BitmapImage barcode;
        private Deal walletDeal;       

        /// <summary>
        /// Deal ID
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                if (value != id)
                {
                    id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        /// <summary>
        /// Deal Title
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// Deal Description
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (value != description)
                {
                    description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Deal Expiration Date
        /// </summary>
        public DateTime ExpirationDate
        {
            get
            {
                return expirationDate;
            }
            set
            {
                if (value != expirationDate)
                {
                    expirationDate = value;
                    NotifyPropertyChanged("ExpirationDate");
                }
            }
        }

        /// <summary>
        /// Deal Terms
        /// </summary>
        public string Terms
        {
            get
            {
                return terms;
            }
            set
            {
                if (value != terms)
                {
                    terms = value;
                    NotifyPropertyChanged("Terms");
                }
            }
        }

        /// <summary>
        /// Deal Code
        /// </summary>
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                if (value != code)
                {
                    code = value;
                    NotifyPropertyChanged("Code");
                }
            }
        }

        /// <summary>
        /// Barcode
        /// </summary>
        public BitmapImage Barcode
        {
            get
            {
                if (barcode == null)
                {
                    barcode = new BitmapImage();
                    barcode.SetSource(Application.GetResourceStream(new Uri("Assets/barcode.bmp", UriKind.Relative)).Stream);
                }
                return barcode;
            }
            set
            {
                if (value != barcode)
                {
                    barcode = value;
                    NotifyPropertyChanged("Code");
                }
            }
        }

        /// <summary>
        /// Wallet deal object
        /// </summary>
        public Deal WalletDeal
        {
            get
            {
                return walletDeal;
            }
            set
            {
                if (value != walletDeal)
                {
                    walletDeal = value;
                    NotifyPropertyChanged("WalletDeal");
                }
            }
        }

        /// <summary>
        /// Wallet status string
        /// </summary>
        public string WalletStatus
        {
            get
            {
                return walletStatus;
            }
            set
            {
                if (value != walletStatus)
                {
                    walletStatus = value;
                    NotifyPropertyChanged("WalletStatus");
                }
            }
        }

        /// <summary>
        /// Event handler fired when there is a property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }  
    }
}

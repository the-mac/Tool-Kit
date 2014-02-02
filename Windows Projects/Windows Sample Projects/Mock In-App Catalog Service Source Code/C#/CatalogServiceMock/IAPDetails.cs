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
using System.Text.RegularExpressions;

namespace CatalogServiceMock
{
    public class IAPDetails
    {
        private static readonly Regex ValidIAPProductID = new Regex(@"^([0-9a-zA-Z\._]){5,255}$", RegexOptions.Compiled);
        private static readonly Regex Validkeyword = new Regex(@"^([0-9a-zA-Z\._]){1,50}$", RegexOptions.Compiled);
        private readonly string[] iapKeywords = new string[10];
        private readonly Guid? iapProductId = Guid.NewGuid(); //new Guid("96ddce32-64f0-46d1-b2f4-05db0e4a2587");
        private readonly Guid? iapSkuId = Guid.NewGuid();
        private readonly Guid? imageId = new Guid("102a19e4-e6f2-495a-acc7-203bb6c742b9");
        private readonly Guid? offerId = Guid.NewGuid();
        private readonly Guid? parentProductId = new Guid("ee29a261-80d0-4bdf-89bd-28b1ebbc8bd3");
        private string iapDescription;
        private string iapId;
        private string iapTag;
        private string iapTitle;
        private string iapType;
        private string price = "0.00";
        private const string priceCurrencyCode = "USD";

        public IAPDetails(string[] array)
        {
            //Keywords = new List<string>();
            KeywordCount = 0;
            //get all keywords
            for (int i = 0; i < array.Length; i++)
            {
                this[KeywordCount] = array[i].Length == 0 ? string.Empty : array[i].Trim();

            }
        }

        public Guid? ParentProductId
        {
            get { return parentProductId; }
        }

        public Guid? IapProductId
        {
            get { return iapProductId; }
        }

        public List<string> Keywords { get; set; }

        // IAP product Identifier
        public string IapId
        {
            get { return iapId; }
            set
            {
                if ((value.Length > 255) && (value.Length > 5))
                {
                    throw new InvalidOperationException("Iap Product ID cannot be greater than 255 characters");
                }
                
                //check for characters used
                iapId = ValidIAPProductID.IsMatch(value) ? value : null;

                if (iapId == null)
                {
                    throw new InvalidOperationException(@"The in-app product identifier: Invalid Format has some characters that aren't allowed. You can use only ""_"" (underscore), ""."" (period), letters A-Z (both upper and lowercase), and numbers.
                     * Review and fix any issues, and then try again");
                }
            }
        }

        public string IapTitle
        {
            get { return iapTitle; }
            set
            {
                if (value.Length > 50)
                {
                    throw new InvalidOperationException("Iap Title cannot be greater than 50 characters");
                }
                
                iapTitle = value;
            }
        }

        public string IapDescription
        {
            get { return iapDescription; }
            set
            {
                if (value.Length > 2000)
                {
                    throw new InvalidOperationException("Iap Description cannot be greater than 2000 characters");
                }

                iapDescription = value;
            }
        }

        public string IapType
        {
            get { return iapType; }

            set
            {
                if ((value.Equals("IAPConsumable", StringComparison.InvariantCultureIgnoreCase) != true) && (value.Equals("IAPDurable", StringComparison.InvariantCultureIgnoreCase) != true))
                {
                    throw new InvalidOperationException("IAP Type needs to be either IAPConsumable or IAPDurable");
                }

                iapType = value;
            }
        }

        public string IapTag
        {
            get { return iapTag; }
            set
            {
                if (value.Length > 3000)
                {
                    throw new InvalidOperationException("Tag Value cannot be greater than 3000 characters");
                }
                
                iapTag = value;
            }
        }

        public Guid? ImageId
        {
            get { return imageId; }
        }

        public string this[int pos]
        {
            get { return iapKeywords[pos]; }

            set
            {
                //empty keywords are not allowed.
                if (value == string.Empty)
                {
                    return;
                }

                if (pos > 10)
                {
                    throw new InvalidOperationException("Keyword count cannot be greater than 10");
                }

                iapKeywords[pos] = (value.Length < 50 && Validkeyword.IsMatch(value)) ? value : null;

                if (iapKeywords[pos] == null)
                    throw new InvalidOperationException(value.Length > 50 ? @"Keyword length needs to be less than 50" : @"Keyword Invalid Format has some characters that aren't allowed. You can use only, letters A-Z (both upper and lowercase), and numbers.
                     * Review and fix any issues, and then try again");

                KeywordCount++;
            }
        }

        public int KeywordCount { get; set; }

        public Guid? OfferId
        {
            get { return offerId; }
        }

        public Guid? IapSkuId
        {
            get { return iapSkuId; }
        }

        public string Price
        {
            get { return price; }

            set { price = value.Length != 0 ? value : "0.00"; }
        }

        public string DisplayPrice { get; set; }

        public string PriceCurrencyCode
        {
            get { return priceCurrencyCode; }
        }
    }
}

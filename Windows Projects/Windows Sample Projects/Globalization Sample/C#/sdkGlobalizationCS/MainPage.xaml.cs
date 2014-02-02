/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Globalization;
using System.Threading;

namespace sdkGlobalizationCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;
        }

        private void LocList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // Set the current culture according to the selected locale and display information such as
            // date, time, currency, etc in the appropriate format.

            string nl;
            string cul;

            nl = locList.SelectedIndex.ToString();

            switch (nl)
            {
                case "0":
                    cul = "zh-CN";
                    break;
                case "1":
                    cul = "zh-TW";
                    break;
                case "2":
                    cul = "cs-CZ";
                    break;
                case "3":
                    cul = "da-DK";
                    break;
                case "4":
                    cul = "nl-NL";
                    break;
                case "5":
                    cul = "en-GB";
                    break;
                case "6":
                    cul = "en-US";
                    break;
                case "7":
                    cul = "fi-FI";
                    break;
                case "8":
                    cul = "fr-FR";
                    break;
                case "9":
                    cul = "de-DE";
                    break;
                case "10":
                    cul = "el-GR";
                    break;
                case "11":
                    cul = "hu-HU";
                    break;
                case "12":
                    cul = "it-IT";
                    break;
                case "13":
                    cul = "ja-JP";
                    break;
                case "14":
                    cul = "ko-KR";
                    break;
                case "15":
                    cul = "nb-NO";
                    break;
                case "16":
                    cul = "pl-PL";
                    break;
                case "17":
                    cul = "pt-BR";
                    break;
                case "18":
                    cul = "pt-PT";
                    break;
                case "19":
                    cul = "ru-RU";
                    break;
                case "20":
                    cul = "es-ES";
                    break;
                case "21":
                    cul = "sv-SE";
                    break;
                default:
                    cul = "en-US";
                    break;
            }


            // set this thread's current culture to the culture associated with the selected locale
            CultureInfo newCulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = newCulture;

            CultureInfo cc, cuic;
            cc = Thread.CurrentThread.CurrentCulture;
            cuic = Thread.CurrentThread.CurrentUICulture;

            // display the culture name in the language of the selected locale
            regionalFrmt.Text = cc.NativeName;

            // display the culture name in the localized language
            displayLang.Text = cuic.DisplayName;

            // display the date formats (long and short form) for the  current culuture  
            DateTime curDate = DateTime.Now;
            longDate.Text = cc.DateTimeFormat.LongDatePattern.ToString() + " " + curDate.ToString("D");
            shortDate.Text = cc.DateTimeFormat.ShortDatePattern.ToString() + "   " + curDate.ToString("d");

            // display the time format (long form) for the current culture
            longTime.Text = cc.DateTimeFormat.LongTimePattern + "   " + curDate.ToString("T");

            // display the currency format and currency symbol for the current culture
            Int64 money = 123456789;
            currencyFrmt.Text = money.ToString("C");
        }

    }
}

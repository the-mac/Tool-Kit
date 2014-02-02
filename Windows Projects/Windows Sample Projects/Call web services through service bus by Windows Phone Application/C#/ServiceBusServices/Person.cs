/****************************** Module Header ******************************\
Module Name:  Person.cs
Project:      ServiceBusServices
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to expose an on-premise REST service to Internet
via Service Bus, then you can access this service by Windows Phone application.
The service includes normal string, generics and image methods.

This is Person model class with some basic properties.

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

namespace ServiceBusServices
{
    public class Person
    {
        public string Name
        {
            get;
            set;
        }

        public string Comments
        {
            get;
            set;
        }
    }
}

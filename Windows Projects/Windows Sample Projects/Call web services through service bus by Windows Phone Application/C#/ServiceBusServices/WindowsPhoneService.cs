/****************************** Module Header ******************************\
Module Name:  WindowsPhoneService.cs
Project:      ServiceBusServices
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to expose an on-premise REST service to Internet
via Service Bus, then you can access this service by Windows Phone application.
The service includes normal string, generics and image methods.

This is the service class.

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
using System.IO;

namespace ServiceBusServices
{
    public class WindowsPhoneService : IWindowsPhoneService
    {
        /// <summary>
        /// Return hello string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Hello(string name)
        {
            return "Hello," + name;
        }

        /// <summary>
        /// Return some entities.
        /// </summary>
        /// <returns></returns>
        public List<Person> Persons()
        {
            Person person = new Person();
            person.Name = "John";
            person.Comments = "John's comments";
            List<Person> list = new List<Person>();
            Person person2 = new Person();
            person2.Name = "John2";
            person2.Comments = "John2's comments";
            list.Add(person2);
            list.Add(person);
            return list;
        }

        /// <summary>
        /// Return MSDN.jpg file stream.
        /// </summary>
        /// <returns></returns>
        public Stream Image()
        {
            FileStream file = File.OpenRead("Files/Microsoft.jpg");
            return file;
        }
    }
}

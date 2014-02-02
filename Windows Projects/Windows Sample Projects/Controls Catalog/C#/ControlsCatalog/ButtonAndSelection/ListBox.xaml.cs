/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

namespace ControlsCatalog.LayoutAndGrouping
{
    public partial class ListBoxDemo : PhoneApplicationPage
    {
        public ListBoxDemo()
        {
            InitializeComponent();
        }

    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }

        public Person(string firstName, string lastName, string streetAddress)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.StreetAddress = streetAddress;
        }
    }

    public class People : ObservableCollection<Person>
    {
        public People()
        {
            Add(new Person("John", "Smith", "123 Maple Ave"));
            Add(new Person("Mary", "Jane", "456 15th St"));
            Add(new Person("Robert", "Lee", "7890 Broadway"));
        }

    }

}

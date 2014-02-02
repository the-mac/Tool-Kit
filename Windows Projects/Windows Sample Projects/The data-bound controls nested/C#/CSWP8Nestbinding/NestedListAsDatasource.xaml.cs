/****************************** Module Header ******************************\
* Module Name:    NestedListAsDatasource.xaml.cs
* Project:        CSWP8Nestbinding
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to nest data-bound controls in WP.
* This page uses nested list as data source.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;

namespace CSWP8Nestbinding
{
    public partial class NestedListAsDatasource : PhoneApplicationPage
    {
        List<Department> listDepartment = new List<Department>();

        public NestedListAsDatasource()
        {
            InitializeComponent();
          
            // Test data.
            for (int i = 1; i < 6; i++)
            {
                List<Person> listPerson = new List<Person>();
                for (int j = 1; j < i + 1; j++)
                {
                    listPerson.Add(new Person("Person" + j, 20 + j));
                }

                listDepartment.Add(new Department("Department" + i, i, listPerson));
            }

            Loaded += NestedListAsDatasource_Loaded;
        }

        void NestedListAsDatasource_Loaded(object sender, RoutedEventArgs e)
        {
            outListBox.ItemsSource = listDepartment;
        }
    }

    /// <summary>
    /// Department entity.
    /// </summary>
    public class Department
    {
        public Department(string strName, int intCount, List<Person> listData)
        {
            this.Name = strName;
            this.PersonCount = intCount;
            this.ListPerson = listData;
        }

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int personCount;
        public int PersonCount
        {
            get { return personCount; }
            set { personCount = value; }
        }

        /// <summary>
        /// List of person.
        /// </summary>
        List<Person> listPerson = new List<Person>();
        public List<Person> ListPerson
        {
            get { return listPerson; }
            set { listPerson = value; }
        }
    }

    /// <summary>
    /// Person entity.
    /// </summary>
    public class Person
    {
        public Person(string strName, int intAge)
        {
            this.Name = strName;
            this.Age = intAge;
        }

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int age;
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
    }
}
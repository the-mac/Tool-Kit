// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeopleHub
{
    public class PeopleViewModel : INotifyPropertyChanged
    {
        private List<Person> _people;

        public PeopleViewModel()
        {
            this.People = new List<Person>();

            this.LoadData();
        }

        /// <summary>
        /// A collection for Person objects.
        /// </summary>
        public List<Person> People
        {
            get
            {
                return _people;
            }
            private set
            {
                _people = value;
                NotifyPropertyChanged();                
            }
        }

        /// <summary>
        /// A collection for Person objects grouped by their first character.
        /// </summary>
        public List<AlphaKeyGroup<Person>> GroupedPeople
        {
            get
            {
                return AlphaKeyGroup<Person>.CreateGroups(
                    People,                
                    (Person s) => { return s.Name; },
                    true);
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few Person objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.People = DataService.GetRandomPeopleList(100);

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
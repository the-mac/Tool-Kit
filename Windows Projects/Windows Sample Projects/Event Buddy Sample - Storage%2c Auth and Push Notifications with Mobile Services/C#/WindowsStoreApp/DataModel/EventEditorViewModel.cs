// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.DataModel
{
    using EventBuddy.Common;

    public class EventEditorViewModel : BindableBase
    {
        private bool editing;

        private Event eventModel;

        public EventEditorViewModel()
        {
            this.Editing = false;
            this.Event = new Event();
        }

        public bool Editing
        {
            get
            { 
                return this.editing;
            }

            set
            {
                if (value != this.editing)
                {
                    this.SetProperty(ref this.editing, value);
                }
            }
        }

        public Event Event
        {
            get { return this.eventModel; }
            set { this.SetProperty(ref this.eventModel, value); }
        }
    }
}

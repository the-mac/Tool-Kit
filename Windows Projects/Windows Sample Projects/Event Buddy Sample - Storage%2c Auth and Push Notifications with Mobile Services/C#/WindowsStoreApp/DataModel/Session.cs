// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.DataModel
{
    using System;
    using EventBuddy.Common;
    using Newtonsoft.Json;

    public class Session : BindableBase
    {
        private string img;
        private string deckSource;
        private string name;

        public Session()
        {
            this.Description = string.Empty;
            this.Room = string.Empty;
            this.Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 30, 0);
            this.End = this.Start.AddHours(1);
        }

        public Session(Event parent)
        {
            this.EventId = parent.Id;

            this.Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 30, 0);
            this.End = this.Start.AddHours(1);
            this.Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum nostrud ipsum consectetur.";
            this.Room = "B33";
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "eventId")]
        public string EventId { get; set; }

        [JsonProperty(PropertyName = "speaker")]
        public string Speaker { get; set; }

        [JsonProperty(PropertyName = "room")]
        public string Room { get; set; }

        [JsonProperty(PropertyName = "start")]
        public DateTime Start { get; set; }

        [JsonProperty(PropertyName = "end")]
        public DateTime End { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string Img
        {
            get
            {
                if (this.img == null)
                {
                    return "Assets/NoProfile.png";
                }

                return this.img;
            }

            set
            {
                this.SetProperty(ref this.img, value);
            }
        }

        [JsonProperty(PropertyName = "deckSource")]
        public string DeckSource
        {
            get
            {
                if (this.deckSource == null)
                {
                    return string.Empty;
                }

                return this.deckSource;
            }

            set
            {
                this.SetProperty(ref this.deckSource, value);
            }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get
            {
                if (this.name == null)
                {
                    return string.Empty;
                }

                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value);
            }
        }
    }
}

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.WindowsPhone.Model
{
    using System;
    using Newtonsoft.Json;

    public class Session
    {
        private string deckSource = string.Empty;

        public Session()
        {
            this.Start = this.End = DateTime.Now;
        }

        public Session(Event parent)
        {
            this.EventId = parent.Id;
            this.Start = this.End = parent.Start;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "eventId")]
        public string EventId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

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
        public string Img { get; set; }

        [JsonProperty(PropertyName = "deckSource")]
        public string DeckSource
        {
            get 
            {
                return this.deckSource; 
            }

            set 
            { 
                this.deckSource = value; 
            }
        }
    }
}

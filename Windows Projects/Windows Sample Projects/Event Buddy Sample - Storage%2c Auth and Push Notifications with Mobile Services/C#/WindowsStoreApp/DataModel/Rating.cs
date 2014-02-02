// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.DataModel
{
    using EventBuddy.Common;
    using Newtonsoft.Json;

    public class Rating : BindableBase
    {
        private string id;
        private string sessionId;
        private float score;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId
        {
            get { return this.sessionId; }
            set { this.SetProperty(ref this.sessionId, value); }
        }

        [JsonProperty(PropertyName = "score")]
        public float Score
        {
            get { return this.score; }
            set { this.SetProperty(ref this.score, value); }
        }
    }
}

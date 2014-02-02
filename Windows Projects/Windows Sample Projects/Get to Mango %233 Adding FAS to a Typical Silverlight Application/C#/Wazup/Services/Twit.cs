using System;
using System.Runtime.Serialization;
using System.Net;

namespace Wazup.Services
{
    /// <summary>
    /// Model for twit
    /// </summary>
    [DataContract]
    public class Twit
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// Gets the decoded text.
        /// </summary>
        /// <value>The decoded text.</value>
        public string DecodedText 
        {
            get
            {
                return HttpUtility.HtmlDecode(text);
            }
        }

        /// <summary>
        /// Gets or sets the from_user.
        /// </summary>
        /// <value>The from_user.</value>
        [DataMember]
        public string from_user { get; set; }

        /// <summary>
        /// Gets or sets the profile_image_url.
        /// </summary>
        /// <value>The profile_image_url.</value>
        [DataMember]
        public string profile_image_url { get; set; }

        /// <summary>
        /// Gets or sets the created_at.
        /// </summary>
        /// <value>The created_at.</value>
        [DataMember]
        public DateTime created_at { get; set; }
    }

    /// <summary>
    /// Model for twitter results
    /// </summary>
    [DataContract]
    public class TwitterResults
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        [DataMember]
        public Twit[] results { get; set; }
    }
}


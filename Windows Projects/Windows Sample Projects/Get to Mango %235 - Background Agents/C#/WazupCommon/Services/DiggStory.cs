using System.Runtime.Serialization;

namespace Wazup.Services
{
    /// <summary>
    /// Model for Digg Story
    /// </summary>
    [DataContract]
    public class DiggStory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiggStory"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="link">The link.</param>
        /// <param name="diggs">The diggs.</param>
        public DiggStory(string title, string description, string link, int diggs)
        {
            Title = title;
            Description = description;
            Link = link;
            Diggs = diggs;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>The link.</value>
        [DataMember]
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the diggs.
        /// </summary>
        /// <value>The diggs.</value>
        [DataMember]
        public int Diggs { get; set; }
    }
}

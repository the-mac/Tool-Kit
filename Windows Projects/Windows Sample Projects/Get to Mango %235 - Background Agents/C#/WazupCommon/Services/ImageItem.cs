using System.Runtime.Serialization;

namespace Wazup.Services
{
    /// <summary>
    /// Model for image item
    /// </summary>
    [DataContract]
    public class ImageItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageItem"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public ImageItem(string fileName)
        {
            FileName = fileName;
            Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
    }
}

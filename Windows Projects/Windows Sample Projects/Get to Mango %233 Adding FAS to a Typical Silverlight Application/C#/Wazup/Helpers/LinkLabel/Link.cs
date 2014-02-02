namespace System.Windows.Controls
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents a link within a LinkLabel control
    /// </summary>
    internal class Link
    {
        private string targetName = "_self";
        private string text;

        /// <summary>
        /// Create new link with key and navigateUri pair
        /// </summary>
        /// <param name="key">The text to be replaced with a link</param>
        /// <param name="navigateUri">The URI link points to</param>
        public Link( string key, Uri navigateUri )
        {
            this.Key = key;
            this.NavigateUri = navigateUri;
        }

        /// <summary>
        /// Create new link with key, navigateUri and action
        /// </summary>
        /// <param name="key">The text to be replaced with a link</param>
        /// <param name="navigateUri">The URI link points to</param>
        /// <param name="action">Action to be taken when the link is clicked</param>
        public Link( string key, Uri navigateUri, string action )
            : this( key, navigateUri )
        {
            this.Action = action;
        }

        /// <summary>
        /// Gets or sets the text to be replaced with a link
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URI to navigate to when the link is clicked
        /// </summary>
        [TypeConverter( typeof( UriTypeConverter ) )]
        public Uri NavigateUri
        {
            get;
            set;
        }

        public string Action
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of a target window or frame to navigate to within the Web page specified by the NavigateUri property
        /// </summary>
        public string TargetName
        {
            get
            {
                return this.targetName;
            }

            set
            {
                this.targetName = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the link. If not specified the Key property is used.
        /// </summary>
        public string Text
        {
            get
            {
                if ( string.IsNullOrEmpty( this.text ) )
                {
                    return this.Key;
                }

                return this.text;
            }

            set
            {
                this.text = value;
            }
        }
    }
}

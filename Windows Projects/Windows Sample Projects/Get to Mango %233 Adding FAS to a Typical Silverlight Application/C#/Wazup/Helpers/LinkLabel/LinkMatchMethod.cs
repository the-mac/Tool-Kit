namespace System.Windows.Controls
{
    public enum LinkMatchMethod
    {
        /// <summary>
        /// Match links by LinkPattern
        /// </summary>
        ByLinkPattern,

        /// <summary>
        /// Match only valid URIs
        /// </summary>
        ByUriPattern,

        /// <summary>
        /// Match valid URIs and links by LinkPattern
        /// </summary>
        ByUriAndLinkPattern
    }
}

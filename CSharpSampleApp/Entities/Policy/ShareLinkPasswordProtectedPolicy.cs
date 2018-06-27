namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible shareable link policies
    /// </summary>
    public enum ShareLinkPasswordProtectedPolicy : short 
    {
        /// <summary>
        /// Default policy
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Do not require user to create password for share file links
        /// </summary>
        Disabled = 1,
        /// <summary>
        /// Require user to create password for all share file links
        /// </summary>
        Enabled = 2
    }
}
namespace CSharpSampleApp.Entities
{

    /// <summary>
    /// An enumeration of possible shareable link policies
    /// </summary>
    public enum ShareableLinkPolicy : short 
    {
        /// <summary>
        /// Default policy
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Disable shareable links (NOT IMPLEMENTED)
        /// </summary>
        DisallowAll = 1,

        /// <summary>
        /// Links can only be shared with folks in the same company
        /// </summary>
        InternalOnly = 2,

        /// <summary>
        /// Allow sharing with anyone (Consumer behavior)
        /// </summary>
        AllowAll = 3
    }
}

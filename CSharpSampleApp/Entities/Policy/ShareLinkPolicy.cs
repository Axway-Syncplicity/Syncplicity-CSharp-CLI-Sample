namespace CSharpSampleApp.Entities
{
    public enum ShareLinkPolicy : short 
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
        AllowAll = 3,
        /// <summary>
        /// Links can only be shared with list of intended users and groups
        /// </summary>
        IntendedOnly = 4 
    }
}

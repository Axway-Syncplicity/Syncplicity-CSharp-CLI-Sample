namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Password complexity options
    /// </summary>
    public enum ShareLinkPasswordComplexity
    {
        // <summary>
        /// Default value
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Password need to be of configirable length and can contain anything
        /// </summary>
        Default = 1,
        /// <summary>
        /// Password need at least one uppercase, one digit and one special character
        /// </summary>
        Complex = 2
    }
}
namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible update policies for a company
    /// </summary>
    public enum ClientAutoUpdatePolicy : short 
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Don't notify users about any updates
        /// </summary>
        None = 1,
        /// <summary>
        /// Only notify users about critical (non-optional) updates
        /// </summary>
        CriticalOnly = 2,
        /// <summary>
        /// Notify users about all updates
        /// </summary>
        All = 3,
    }
}

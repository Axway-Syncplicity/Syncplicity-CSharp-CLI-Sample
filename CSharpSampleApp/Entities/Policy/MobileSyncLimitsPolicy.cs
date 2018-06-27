namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible mobile sync limits policies for a company
    /// </summary>
    public enum MobileSyncLimitsPolicy : byte
    {
        /// <summary>
        /// Default policy
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Allow users to set their own limits on how much data can be transferred over a cellular network every billing cycle
        /// </summary>
        None = 1,
        /// <summary>
        /// Enforce the special limits on how much data can be transferred over a cellular network (note: synchronization over a Wi-Fi hotspot will remain unrestricted)
        /// </summary>
        Enforce = 2
    }
}

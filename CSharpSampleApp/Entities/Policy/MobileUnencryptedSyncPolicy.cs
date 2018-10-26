namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible mobile unencrypted sync policies for a company
    /// </summary>
    public enum MobileUnencryptedSyncPolicy : byte
    {
        /// <summary>
        /// Default Policy
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Disallow unencrypted sync and require users to only sync to an encrypted cache managed by Syncplicity
        /// </summary>
        Disabled = 1,

        /// <summary>
        /// Allow users to synchronize files and folders to an SD card where they reside unencrypted
        /// </summary>
        Enabled = 2
    }
}

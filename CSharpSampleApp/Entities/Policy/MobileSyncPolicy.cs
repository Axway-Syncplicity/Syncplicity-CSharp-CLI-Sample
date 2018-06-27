namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible mobile sync policies for a company
    /// </summary>
    public enum MobileSyncPolicy : byte
    {
        /// <summary>
        /// Default policy
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Users can synchronize files and folders to their iOS and Android devices on any network
        /// </summary>
        All = 1,
        /// <summary>
        /// Users can synchronize files and folders to their iOS and Android devices, but only when the device is connected to a Wi-Fi hotspot (synchronization on 3G/4G/WiMax is disallowed)
        /// </summary>
        WiFiOnly = 2,
        /// <summary>
        /// Users can synchronize files and folders to their iOS and Android devices, but only when the device is connected to a home network (i.e. not roaming) or a Wi-Fi hotspot
        /// </summary>
        WiFiCellular = 3,
        /// <summary>
        /// Automatic synchronization is disabled on all mobile devices
        /// </summary>
        Disabled = 4
    }
}

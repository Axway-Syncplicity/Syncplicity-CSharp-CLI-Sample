namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible remote wipe syncpoint policies for a company
    /// </summary>
    public enum RemoteWipeSyncpointPolicy
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Don't remote wipe a syncpoint when a user looses access
        /// </summary>
        Disabled = 1,

        /// <summary>
        /// Remote wipe a syncpoint when a user looses access
        /// </summary>
        Enabled = 2
    }
}

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible update policies for a company
    /// </summary>
    public enum ClientPreconfiguredPolicy : short 
    {
        /// <summary>
        /// Nothing is known about the pre-configuration policy for this company
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// All machines for this company are pre-configured
        /// </summary>
        AllMachinesArePreconfigured = 1,

        /// <summary>
        /// No machines for this company are pre-configured
        /// </summary>
        NoMachinesArePreconfigured = 2,

        /// <summary>
        /// Some machines are pre-configured (NOT IMPLEMENTED)
        /// </summary>
        SomeMachinesArePreconfigured = 3
    }
}

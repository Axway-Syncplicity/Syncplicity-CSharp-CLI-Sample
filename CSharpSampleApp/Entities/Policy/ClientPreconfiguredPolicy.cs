namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible update policies for a company
    /// </summary>
    public enum ClientPreconfiguredPolicy : short 
    {
        /// <summary>
        /// Nothing is known about the preconfiguration policy for this company
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// All machines for this company are preconfigured
        /// </summary>
        AllMachinesArePreconfigured = 1,
        /// <summary>
        /// No machines for this company are preconfigured
        /// </summary>
        NoMachinesArePreconfigured = 2,
        /// <summary>
        /// Some machines are preconfigured (NOT IMPLEMENTED)
        /// </summary>
        SomeMachinesArePreconfigured = 3
    }
}

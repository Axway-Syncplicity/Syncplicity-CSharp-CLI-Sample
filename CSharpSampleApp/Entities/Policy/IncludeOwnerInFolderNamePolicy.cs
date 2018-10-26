namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible include owner policies for a company
    /// </summary>
    public enum IncludeOwnerInFolderNamePolicy : short
    {
        /// <summary>
        /// Nothing is known about the include owner policy for this company
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Owner name is included
        /// </summary>
        Included = 1,

        /// <summary>
        /// Owner name is ignored
        /// </summary>
        Ignored = 2,
    }
}

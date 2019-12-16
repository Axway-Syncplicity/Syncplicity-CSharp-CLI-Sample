namespace CSharpSampleApp.Entities
{
    public enum IrmRoleType : short
    {
        /// <summary>
        ///  Default value
        /// </summary>
        None = 0,
        /// <summary>
        /// Read only role
        /// </summary>
        Reader = 1,
        /// <summary>
        /// Read and Write role
        /// </summary>
        Editor = 2
    }
}

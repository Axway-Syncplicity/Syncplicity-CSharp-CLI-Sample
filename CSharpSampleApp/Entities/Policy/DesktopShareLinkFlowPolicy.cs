namespace CSharpSampleApp.Entities
{
    public enum DesktopShareLinkFlowPolicy : byte
    {
        /// <summary>
        /// Default policy
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Display the share link directly in place
        /// </summary>
        InPlace = 1,

        /// <summary>
        /// Take the user to My site
        /// </summary>
        RedirectToMySite = 2
    }
}

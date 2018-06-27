namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Grant types enum.
    /// </summary>
    public enum GrantType : byte
    {
        AuthorizationCode = 1,
        Implicit = 2,
        Password = 3
    }
}
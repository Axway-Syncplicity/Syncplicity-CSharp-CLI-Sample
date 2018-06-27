namespace CSharpSampleApp.Entities.PII
{
    public class PII
    {
        public RolUserPII[] RolUserPIIs { get; set; }
    }

    public class RolUserPII
    {
        public User[] Users { get; set; }
        public int HomeRolId { get; set; }
        public string Bearer { get; set; }
    }
}

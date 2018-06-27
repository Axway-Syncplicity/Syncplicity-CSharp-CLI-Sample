using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class LinkUsage
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public User User;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public int NumDownloads;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public DateTime LastDownloadDateUtc;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string LastDownloadIpAddress;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public decimal? LastDownloadGeoLatitude;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public decimal? LastDownloadGeoLongitude;
    }
}
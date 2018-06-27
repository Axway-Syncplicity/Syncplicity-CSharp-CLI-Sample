using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class File
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public long SyncpointId;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public long FileId;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string VirtualPath;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string Filename;

        [DataMember(EmitDefaultValue = true, Order = 4)]
        public long Length;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public string Hash;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public string CreationTimeUtc;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public string LastWriteTimeUtc;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public byte SyncPriority;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public FileStatus Status;

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public long LatestVersionId;

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public FileVersion[] Versions;

        [DataMember(EmitDefaultValue = false, Order = 12)]
        public bool Stored;

        [DataMember(EmitDefaultValue = false, Order = 13)]
        public string ThumbnailUrl;

        [DataMember(EmitDefaultValue = true, Order = 14)]
        public long FolderId;

        /// <summary>
        /// Returns a logging-friendly string
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string ToLoggingString()
        {
            return string.Format(
                "Virtual Path: {0}, Filename: {1}, Status: {2}, VirtualFolderId: {3}, LatestVersionId: {4}",
                VirtualPath,
                Filename,
                Status.ToString(),
                SyncpointId,
                LatestVersionId);
        }
    }
}
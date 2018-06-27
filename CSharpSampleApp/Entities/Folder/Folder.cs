using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Folder
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public long SyncpointId;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public long FolderId;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string VirtualPath;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string Name;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public FolderStatus Status;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public File[] Files;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public Folder[] Folders;

        /// <summary>
        /// Returns a logging-friendly string
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string ToLoggingString()
        {
            return string.Format(
                "Virtual Path: {0}, Name: {1}, Status: {2}, VirtualFolderId: {3}, DataFolderId: {4}",
                VirtualPath,
                Name,
                Status.ToString(),
                SyncpointId,
                FolderId);
        }
    }
}
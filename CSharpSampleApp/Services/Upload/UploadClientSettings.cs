using System;

namespace CSharpSampleApp.Services.Upload
{
    public enum FileBinaryDataBodyTag : short
    {
        FileData = 0,
        File = 1
    }

    public class UploadClientSettings
    {
        /// <summary>
        /// Gets or sets the use query. By default data send with form params
        /// Parameters will be add to query if set to TRUE:
        ///     - CreateTimeUtc
        ///     - LastWriteTimeUtc
        ///     - sha256
        ///     - sessionKey
        ///     - virtualFolderId
        ///     - fileDone
        /// </summary>
        /// <value>
        /// The use query.
        /// </value>
        public Boolean UseQueryParams { get; set; }

        /// <summary>
        /// Gets or sets the body file data tag.
        /// </summary>
        /// <value>
        /// The body file data tag.
        /// </value>
        public FileBinaryDataBodyTag FileBinaryDataBodyTag { get; set; }

        public bool AddCreationTimeToRequest { get; set; }

        public bool AddLastWriteTimeToRequest { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadClientSettings" /> class.
        /// </summary>
        public UploadClientSettings()
        {
            UseQueryParams = false;
            FileBinaryDataBodyTag = FileBinaryDataBodyTag.FileData;
            AddCreationTimeToRequest = true;
            AddLastWriteTimeToRequest = true;
        }
    }
}
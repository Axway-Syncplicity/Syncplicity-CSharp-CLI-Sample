using System;
using System.Configuration;

namespace CSharpSampleApp.Util
{
    /// <summary>
    /// Configuration helper class to work with application configuration.
    /// </summary>
    public static class ConfigurationHelper
    {
        #region Define Constants

        /* Should not need to edit these values */
        private const string DefaultGolUrl = "https://gol.dev.syncplicity.com";

        private const string DefaultSimplePassword = "123123aA";
        private const string DefaultReportsFolder = "Syncplicity Reports"; //Default system name, should not need to change
        private const string DefaultGroupName = "SampleAppGroup-";
        private const string DefaultSyncpointName = "SampleAppSyncpoint-";
        private const string DefaultFolderName = "SampleAppFolder-";
        private const string DefaultReportName = "SampleAppReportStorageByUser-";

        #endregion Define Constants

        #region Public Properties

        /// <summary>
        /// Global orchestration layer URL
        /// </summary>
        public static string GolUrl => ConfigurationManager.AppSettings["golUrl"] ?? DefaultGolUrl;


        /// <summary>
        /// Configuration value of OAuth consumer key.
        /// </summary>
        public static string ApplicationKey => GetSettingsValueWithoutPlaceholder("appKey", "REPLACE_WITH_APP_KEY");

        /// <summary>
        /// Configuration value of OAuth consumer secret phrase.
        /// </summary>
        public static string ApplicationSecret => GetSettingsValueWithoutPlaceholder("appSecret", "REPLACE_WITH_APP_SECRET");

        /// <summary>
        /// Configuration value of Syncplicity Admin Application Token.
        /// </summary>
        /// <remarks>
        /// The syncplicityAdminToken is a value generated inside the
        /// Syncplicity web application admin page (must have admin
        /// access).  It is a user specific application key that
        /// is generated on the main account page: https://my.syncplicity.com/Account/
        /// </remarks>
        public static string SyncplicityAdminToken => GetSettingsValueWithoutPlaceholder("syncplicityAdminToken", "REPLACE_WITH_ADMIN_TOKEN");

        /// <summary>
        /// The owner email should be set to the email created during the initial login to the developer portal.
        /// </summary>
        /// <remarks>
        /// For example, if you initiated the login to the developer portal as foo.bar@baz.com,
        /// then we instantiated a sandbox account with foo.bar-apidev@baz.com as the owner email.
        /// </remarks>
        public static string OwnerEmail => GetSettingsValueWithoutPlaceholder("ownerEmail", "REPLACE_OWNER_EMAIL");

        /// <summary>
        /// The other rol email should be set to email of the user hosted on another rol.
        /// </summary>
        public static string OtherRolEmail => GetSettingsValueWithoutPlaceholder("otherRolEmail", "REPLACE_OTHER_ROL_OWNER_EMAIL");

        /// <summary>
        /// Enter the local path to a small file that is readable and able to be uploaded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensure there is a non-zero amount bytes in the file as 0 length files will not upload properly.
        /// The file size should not exceed 5 Mb. Larger files should use chunked upload.
        /// </para>
        /// <para>
        /// Both full and relative paths are supported. Note: relative paths are resolved from the output .exe file.
        /// On Windows, use \\ instead of \ to denote path separators.
        /// </para>
        /// </remarks>
        /// <example>
        /// C:\\Temp\foo.txt
        /// </example>
        public static string UploadFileSmall => ConfigurationManager.AppSettings.Get("uploadFileSmall");

        /// <summary>
        /// Enter the local path to a large file that is readable and able to be uploaded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The file size should be greater than 5 Mb.
        /// </para>
        /// <para>
        /// Both full and relative paths are supported. Note: relative paths are resolved from the output .exe file.
        /// On Windows, use \\ instead of \ to denote path separators.
        /// </para>
        /// </remarks>
        /// <example>
        /// C:\\Temp\foo.txt
        /// </example>
        public static string UploadFileLarge => ConfigurationManager.AppSettings.Get("uploadFileLarge");

        /// <summary>
        /// Default group name used for creating user groups
        /// </summary>
        public static string GroupName => DefaultGroupName;

        /// <summary>
        /// Default syncpoint name used for creating syncpoints
        /// </summary>
        public static string SyncpointName => DefaultSyncpointName;

        /// <summary>
        /// Default folder name used for creating syncpoints
        /// </summary>
        public static string FolderName => DefaultFolderName;

        /// <summary>
        /// Returns a simple password used for the reporting service
        /// </summary>
        public static string SimplePassword => DefaultSimplePassword;

        /// <summary>
        /// Returns a default directory that can be used to stored the reports
        /// once they're generated by the syncplicity backend.
        /// </summary>
        public static string ReportsFolder => DefaultReportsFolder;

        /// <summary>
        /// Returns the full path to a directory (does not need to exist, program will create it).
        /// The program will download the generated report export file to the specified directory.
        /// </summary>
        public static string ReportDownloadFolder => DownloadFolder;

        /// <summary>
        /// Returns a default report name to be used when scheduling report
        /// creation with the syncplicity reporting service
        /// </summary>
        public static string ReportName => DefaultReportName;


        /// <summary>
        /// Returns the full path to a directory (does not need to exist, program will create it).
        /// The program will download the file to the specified directory.
        /// </summary>
        public static string DownloadFolder => GetSettingsValueWithoutPlaceholder("downloadDirectory", "REPLACE_WITH_DOWNLOAD_DIRECTORY");

        /// <summary>
        /// Email of the user to assign ownership to, in the change ownership sample.
        /// </summary>
        public static string NewSyncpointOwnerEmail => GetSettingsValueWithoutPlaceholder("newSyncpointOwnerEmail", "REPLACE_WITH_NEW_SYNCPOINT_OWNER_EMAIL");

        /// <summary>
        /// Storage token
        /// </summary>
        public static string StorageToken => GetSettingsValueWithoutPlaceholder("storageToken", "REPLACE_WITH_STORAGE_TOKEN");

        /// <summary>
        /// Machine ID
        /// </summary>
        public static Guid MachineId => Guid.Parse (GetSettingsValueWithoutPlaceholder("machineId", "REPLACE_WITH_MACHINE_ID"));

        /// <summary>
        /// Machine token 
        /// </summary>
        public static string SyncplicityMachineToken => GetSettingsValueWithoutPlaceholder("machineToken", "REPLACE_WITH_MACHINE_TOKEN");

        /// <summary>
        /// Determines if machine token authentication is enabled.
        /// </summary>
        public static bool SyncplicityMachineTokenAuthenticationEnabled => !string.IsNullOrEmpty(SyncplicityMachineToken);

        /// <summary>
        /// Email of user on behalf of which requests will be performed.
        /// </summary>
        public static string AsUserEmail => GetSettingsValueWithoutPlaceholder("asUserEmail", "REPLACE_WITH_ONBEHALFOF_USER_EMAIL");

        #endregion

        private static string GetSettingsValueWithoutPlaceholder(string settingKey, string settingsValuePlaceholder)
        {
            var st = ConfigurationManager.AppSettings.Get(settingKey);
            return st == settingsValuePlaceholder ? string.Empty : st;
        }
    }   
}
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
        private const string GOL_URL = "https://gol.dev.syncplicity.com";

        private const string SIMPLE_PASSWORD = "123123aA";
        private const string REPORTS_FOLDER = "Syncplicity Reports"; //Default system name, should not need to change
        private const string GROUP_NAME = "SampleAppGroup-";
        private const string SYNCPOINT_NAME = "SampleAppSyncpoint-";
        private const string FOLDER_NAME = "SampleAppFolder-";
        private const string REPORT_NAME = "SampleAppReportStorageByUser-";

        #endregion Define Constants
        #region Public Properties

        /// <summary>
        /// Global orchestration layer URL
        /// </summary>
        public static string GolUrl
        {
            get { return ConfigurationManager.AppSettings["golUrl"] ?? GOL_URL; }
        }


        /// <summary>
        /// Configuration value of OAuth consumer key.
        /// </summary>
        public static string ApplicationKey
        {
            get { return ConfigurationManager.AppSettings.Get("appKey"); }
        }

        /// <summary>
        /// Configuration value of OAuth consumer secret phrase.
        /// </summary>
        public static string ApplicationSecret
        {
            get { return ConfigurationManager.AppSettings.Get("appSecret"); }
        }

        /// <summary>
        /// Configuration value of Syncplicity Admin Application Token.
        /// The syncplicityAdminToken is a value generated inside the
        /// Syncplicity web application admin page (must have admin
        /// access).  It is a user specific application key that
        /// is generated on the main account page: https://my.syncplicity.com/Account/ 
        /// </summary>
        public static string GetSyncplicityAdminToken(int roleId)
        {
            return ConfigurationManager.AppSettings.Get(string.Format("syncplicityAdminToken-{0}", roleId));
        }

        //The owner email should be set to the email created during the
        //initial login to the developer portal.  For example, if you 
        //initiated the login to the developer portal as foo.bar@baz.com,
        //then we instantiated a sandbox account with foo.bar-apidev@baz.com as
        //the owner email. 	
        public static string OwnerEmail
        {
            get { return ConfigurationManager.AppSettings.Get("ownerEmail"); }
        }

        //The other rol email should be set to email of the user hosted on another rol.
        public static string OtherRolEmail
        {
            get { return ConfigurationManager.AppSettings.Get("otherRolEmail"); }
        }

        //Enter the full path on the local hard drive to a file that is readable
        //and able to be uploaded.  Ensure there is a non-zero amount bytes 
        //in the file as 0 length files will not upload properly.  
        //
        //NOTE: on windows use \\ instead of \ to denote path seperators
        //Examples: C:\\Temp\foo.txt
        public static string UploadFile
        {
            get { return ConfigurationManager.AppSettings.Get("uploadFile"); }
        }

        //Default group name used for creating user groups
        public static string GroupName
        {
            get { return GROUP_NAME; }
        }

        //Default syncpoint name used for creating syncpoints
        public static string SyncpointName
        {
            get { return SYNCPOINT_NAME; }
        }

        //Default folder name used for creating syncpoints
        public static string FolderName
        {
            get { return FOLDER_NAME; }
        }

        // Returns a simple password used for the reporting service
        public static string SimplePassword
        {
            get { return SIMPLE_PASSWORD; }
        }

        // Returns a default directory that can be used to stored the reports
        // once they're genreated by the syncplicity backend.  
        public static string ReportsFolder
        {
            get { return REPORTS_FOLDER; }
        }

        // Returns the full path to a directory (does not need to exist, program will
        // create it).  The program will download the generated report export file
        // to the specified directory.
        public static string ReportDownloadFolder
        {
            get { return ConfigurationManager.AppSettings.Get("downloadDirectory"); }
        }

        // Returns a default report name to be used when scheduling report
        // creation with the syncplicity reporting service
        public static string ReportName
        {
            get { return REPORT_NAME; }
        }       

        
        // Returns the full path to a directory (does not need to exist, program will
        // create it).  The program will download the file
        // to the specified directory.
        public static string DownloadFolder
        {
            get { return ConfigurationManager.AppSettings.Get("downloadDirectory"); }
        }
      
        // e-mail of the user who is the owner of syncpoint 
        public static string NewSyncpointOwnerEmail
        {
            get {
                var st = ConfigurationManager.AppSettings.Get("newSyncpointOwnerEmail");
                if (st == "REPLACE_WITH_NEW_SYNCPOINT_OWNER_EMAIL") return string.Empty;
                return st;
            }
        }

        // storage token 
        public static string StorageToken
        {
            get { return ConfigurationManager.AppSettings.Get("storageToken"); }
        }

          // machine token 
        public static Guid MachineId
        {
            get { return Guid.Parse (ConfigurationManager.AppSettings.Get("machineId")); }
        }

        // machine token 
        public static string SyncplicityMachineToken
        {
            get
            {
                var st = ConfigurationManager.AppSettings.Get("machineToken");
                if (st == "REPLACE_WITH_MACHINE_TOKEN") return string.Empty;

                return st;
            }
        }

        // is machine token authentication enabled
        public static bool SyncplicityMachineTokenAuthenticationEnabled
        {
            get { return !string.IsNullOrEmpty(SyncplicityMachineToken); }
        }
        
        // Email of user on behalf of which requests will be performed
        public static string AsUserEmail
        {
            get {
                var st = ConfigurationManager.AppSettings.Get("asUserEmail");
                if (st == "REPLACE_WITH_ONBEHALFOF_USER_EMAIL") return string.Empty;

                return st;
            }
        }

        #endregion
    }   
}
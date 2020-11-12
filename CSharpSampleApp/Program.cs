using System;
using System.Collections.Generic;
using System.Linq;
using CSharpSampleApp.Examples;
using CSharpSampleApp.Services;
using System.Net;
using CSharpSampleApp.Util;

namespace CSharpSampleApp
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                Initialize();

                Console.WriteLine(@"C# Sample App starting...");
                Console.WriteLine();
                /* 
                 * The sample app will show simplified examples of calls that you can make against the 
                 * api gateway using the available REST calls.
                 * 
                 * The example calls that this app will make include:
                 * 
                 * Authorization
                 * - OAuth authorization call (to allow this app to connect to the gateway and make API calls)
                 * 
                 * Provisioning
                 * - Creating new users associated with a company
                 * - Creating a new user group (a group as defined in Syncplicity as having access to the same shared folders)
                 * - Associating the newly created users with the new user group
                 * 
                 * Content
                 * - Creating a Syncpoint to allow uploads/downloads to folders
                 * - Creating a folder
                 * - Uploading a file into the folder
                 * - Downloading the uploaded file
                 * - Removing the uploaded file
                 * - Removing the folder
                 * - Changing owner of the syncpoint
                 * - Searching files and folders
                 */

                ValidateRequiredConfiguration();

                GolGateway.LoadRols(ConfigurationHelper.OwnerEmail);

                ApiGateway.Authenticate();

                Console.WriteLine();

                if (!ApiContext.Authenticated)
                {
                    Console.WriteLine("The OAuth authentication has failed, the app cannot continue.");

                    Console.WriteLine();
                    Console.WriteLine("Press enter to close...");
                    Console.ReadLine();

                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Authentication was successful. Press enter to continue to Provisioning example.");
                    Console.ReadLine();
                }

                ProvisioningExample.Execute();
                Console.WriteLine();
                Console.WriteLine("Provisioning part is completed. Press enter to continue to simple file upload example.");
                Console.ReadLine();
                
                ContentExample.ExecuteSimple();
                Console.WriteLine();
                Console.WriteLine("Simple upload part is completed. Press enter to continue to chunked file upload example.");
                Console.ReadLine();

                ContentExample.ExecuteChunked();
                Console.WriteLine();
                Console.WriteLine("Chunked upload part is completed. Press enter to continue to create IRM shared links example.");
                Console.ReadLine();

                ContentExample.ExecuteLinksPost();
                Console.WriteLine();
                Console.WriteLine("Create a shared link part is completed. Press enter to continue to Get shared file links example.");
                Console.ReadLine();

                ContentExample.ExecuteLinksGet();
                Console.WriteLine();
                Console.WriteLine("Get shared file links part is completed. Press enter to continue to Get a shared file link metadata example.");
                Console.ReadLine();

                ContentExample.ExecuteLinkTokenGet();
                Console.WriteLine();
                Console.WriteLine("Get shared file links part is completed. Press enter to continue to Update a shared file link example.");
                Console.ReadLine();

                ContentExample.ExecuteLinkTokenPut();
                Console.WriteLine();
                Console.WriteLine("Update a shared file link part is completed. Press enter to continue to Delete a shared file link example.");
                Console.ReadLine();

                ContentExample.ExecuteLinkTokenDelete();
                Console.WriteLine();
                Console.WriteLine("Delete a shared file link part is completed. Press enter to continue to rename folder example.");
                Console.ReadLine();

                ContentExample.ExecuteRenameFolder();
                Console.WriteLine();
                Console.WriteLine("Rename folder part is completed. Press enter to continue to content rename file example.");
                Console.ReadLine();

                ContentExample.ExecuteRenameFile();
                Console.WriteLine();
                Console.WriteLine("Rename file part is completed. Press enter to continue to newsfeed events example.");
                Console.ReadLine();

                ContentExample.ExecuteNewsFeedEvents();
                Console.WriteLine();
                Console.WriteLine("NewsFeed Events part is completed. Press enter to continue to content create nested folders example.");
                Console.ReadLine();

                ContentExample.ExecuteCreateNestedFolder();
                Console.WriteLine();
                Console.WriteLine("Create nested folders part is completed. Press enter to continue to content search example.");
                Console.ReadLine();

                SearchExample.Execute();
                Console.WriteLine();
                Console.WriteLine("Search part is completed. Press enter to continue to folder tagging example.");
                Console.ReadLine();

                ContentExample.ExecuteFolderTagging();
                Console.WriteLine();
                Console.WriteLine("Folder tagging part is completed. Press enter to continue to file tagging example.");
                Console.ReadLine();

                ContentExample.ExecuteFileTagging();
                Console.WriteLine();
                Console.WriteLine("File tagging part is completed. Press enter to continue to on behalf of another user example.");
                Console.ReadLine();

                ContentExample.ExecuteOnBehalfOf();
                Console.WriteLine();
                Console.WriteLine("On-Behalf-Of part is completed. Press enter to continue to legal hold example.");
                Console.ReadLine();

                ContentExample.ExecuteLegalHold();
                Console.WriteLine();
                Console.WriteLine("Content part is completed. Press enter to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Fatal error");
                Console.WriteLine(e);
                Console.WriteLine();

                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
        }

        private static void Initialize()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private static void ValidateRequiredConfiguration()
        {
            var configurationErrors = EvaluateConfigurationValidationRules().ToList();
            if (!configurationErrors.Any()) return;

            Console.Error.WriteLine("Configuration is not valid, no samples can be executed. Please check the error below and correct the App.config file:");
            configurationErrors.ForEach(e => Console.Error.WriteLine($"\tError: {e}"));

            Environment.Exit(1);
        }

        private static IEnumerable<string> EvaluateConfigurationValidationRules()
        {
            var noAppKey = string.IsNullOrWhiteSpace(ConfigurationHelper.ApplicationKey);
            if (noAppKey) yield return "appKey is not specified";

            var noAppSecret = string.IsNullOrWhiteSpace(ConfigurationHelper.ApplicationSecret);
            if (noAppSecret) yield return "appSecret is not specified";

            var noAdminTokenSpecified = string.IsNullOrWhiteSpace(ConfigurationHelper.SyncplicityAdminToken);
            if (noAdminTokenSpecified) yield return "syncplicityAdminToken is not specified";

            var noOwnerEmail = string.IsNullOrWhiteSpace(ConfigurationHelper.OwnerEmail);
            if (noOwnerEmail) yield return "ownerEmail is not specified";
        }
    }
}

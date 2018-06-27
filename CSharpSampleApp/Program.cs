using System;

using CSharpSampleApp.Examples;
using CSharpSampleApp.Services;
using System.Net;
using CSharpSampleApp.Util;

namespace CSharpSampleApp
{
    static class Program
    {
        static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            Console.WriteLine(@"C# Sample App starting...");
            Console.WriteLine();
            /* 
             * The sample app will show simplified examples of calls that you can make against the 
             * api gateway using the available REST calls.
             * 
             * The example calls that this app will make include:
             * 
             * Authorization
             * - OUath authorization call (to allow this app to connect to the gateway and make API calls)
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
             */

            GolGateway.LoadRols(ConfigurationHelper.OwnerEmail);

            OAuth.OAuth.authenticate();

            Console.WriteLine();

            if (!APIContext.Authenticated)
            {
                Console.WriteLine(@"The OAuth authentication has failed, the app cannot continue.");

                Console.WriteLine();
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();

                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine(@"Authentication was successful.");
            }

            ProvisioningExample.execute();

            Console.WriteLine();
            Console.WriteLine("Provisioning part is completed. Press enter to continue.");
            Console.ReadLine();

            CrossRolExample.Execute();
            Console.WriteLine();
            Console.WriteLine("Cross ROL part is completed. Press enter to continue.");
            Console.ReadLine();

            ContentExample.execute();
            ContentExample.executeObo();

            Console.WriteLine();
            Console.WriteLine("Content part is completed. Press enter to exit...");
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections;

using CSharpSampleApp.Entities;
using CSharpSampleApp.Util;
using CSharpSampleApp.Services;


namespace CSharpSampleApp.Examples
{
    public class ProvisioningExample
    {

        private static int NewUsersCount = 5;

        private static User[] createdUsers;
        private static User[] groupMemberUsers;
        private static Group createdGroup;

        /*
         * Provisioning
         * - Creating new users associated with a company
         * - Creating a new user group (a group as defined in Syncplicity as having access to the same shared folders)
         * - Associating the newly created users with the new user group
         * - Removing the users from the group
         * - Deleting the group
         */
        public static void execute()
        {
            createUsers();
            createGroup();
            addUsersToGroup();
            removeUsersFromGroup();
            deleteGroup();
            deleteUsers();
        }

        private static void deleteUsers()
        {
            if (createdUsers == null)
            {
                return;
            }

            Console.WriteLine();

            // remove created users
            Console.WriteLine();
            Console.WriteLine(@"Start Users Deletion...");

            foreach (User user in createdUsers)
            {
                Console.WriteLine(string.Format("\tDelete User With Email {0}", user.EmailAddress));
                UsersService.DeleteUser(user.EmailAddress);
            }

            Console.WriteLine();
            Console.WriteLine("Finished Users Deletion.");
        }

        private static void deleteGroup()
        {
            if (createdGroup == null)
            {
                return;
            }

            Console.WriteLine();

            //Remove the previously created group
            Console.WriteLine();
            Console.WriteLine(String.Format("Delete Group With Id {0}", createdGroup.Id));
            GroupsService.DeleteGroup(createdGroup.Id);
        }

        private static void removeUsersFromGroup()
        {
            if (groupMemberUsers == null || groupMemberUsers.Length == 0 || createdGroup == null)
            {
                return;
            }

            Console.WriteLine();

            // remove selected group member
            Console.WriteLine();
            Console.WriteLine("Start Group Member Deletion...");

            foreach (User user in groupMemberUsers)
            {
                Console.WriteLine();
                Console.WriteLine(String.Format("\tRemoving user {0} from group {1}.", user.EmailAddress, createdGroup.Id));
                GroupMembersService.DeleteGroupMember(
                        createdGroup.Id,
                        user.EmailAddress);
            }

            Console.WriteLine();
            Console.WriteLine("Finish Group Member Deletion.");
        }

        private static void addUsersToGroup()
        {

            if (createdUsers == null || createdUsers.Length == 0 || createdGroup == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Start Adding Group Members...");

            groupMemberUsers = GroupMembersService.AddGroupMembers(
                                  createdGroup.Id,
                                  createdUsers
                               );

            Console.WriteLine();

            if (groupMemberUsers == null || groupMemberUsers.Length == 0)
            {

                int addedCount = (groupMemberUsers == null ? 0 : groupMemberUsers.Length);
                Console.WriteLine(String.Format("Error Occured During Adding Some Of Members. Number of Added Members:{0}", addedCount));

                return;
            }

            Console.WriteLine("Finish Adding Group Members.");
        }

        private static void createGroup()
        {
            Console.WriteLine();

            if (!APIContext.CompanyGuid.HasValue)
            {
                Console.WriteLine(@"Group Can't Be Created Because Company Guid Wasn't Received During Authorization.");
                return;
            }

            Console.WriteLine("Start Group Creation...");

            Random random = new Random();

            Group group = new Group()
            {
                Name = ConfigurationHelper.GroupName + random.Next().ToString(),  //Use timestamp to generate unique name
                Owner = new User() { EmailAddress = ConfigurationHelper.OwnerEmail }
            };

            Group[] createdGroups = GroupsService.CreateGroups(
                                        APIContext.CompanyGuid.Value,
                                        new Group[] { group }
                                    );

            Console.WriteLine();

            if (createdGroups == null || createdGroups.Length == 0)
            {
                Console.WriteLine("Error Occured During Creating Group.");
                return;
            }

            createdGroup = createdGroups[0];

            Console.WriteLine(String.Format("Finished Group Creation. New Group Id: {0}", createdGroup.Id));
        }

        private static void createUsers()
        {
            Random random = new Random();
            String baseEmail = string.Format("fake-user-{0}@dispostable.com", random.Next().ToString().Substring(0, 7));

            // create users with email typed by user + sequence number
            Console.WriteLine();
            Console.WriteLine(@"Start Users Creation...");

            String[] emailParts = baseEmail.Split('@');

            ArrayList usersToAdd = new ArrayList();

            for (int i = 1; i <= NewUsersCount; i++)
            {

                User user = new User()
                {
                    AccountType = AccountType.LimitedBusiness,
                    EmailAddress = String.Format("{0}-{1}@{2}", emailParts[0], i, emailParts[1]),
                    Password = ConfigurationHelper.SimplePassword,
                    FirstName = String.Format("{0}-{1}", emailParts[0], i),
                    LastName = String.Format("Lastname {0}", i)
                };

                usersToAdd.Add(user);
                Console.WriteLine();
                Console.WriteLine(String.Format("\tPreparing User #{0} [{1}].", i, user.EmailAddress));
                Console.WriteLine(String.Format("\tChecking if user [{0}] already exists.", user.EmailAddress));
                User checkUser = UsersService.GetUser(user.EmailAddress);

                Console.WriteLine();

                if (checkUser != null)
                {
                    //If this is the second time running, we'll need to clean up (delete) previous run's users
                    //This is just to keep the sample code working as if it was the first time run...
                    Console.WriteLine(String.Format("\tCleanup of User #{0} [{1}].  Deleting user that may have been created from previous Sample App run.", i, user.EmailAddress));
                    try
                    {
                        UsersService.DeleteUser(user.EmailAddress);
                    }
                    catch (Exception e)
                    {
                        //ignore exceptions as this user may not exist
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("\tNo user found in company, continuing successfully."));
                }
            }

            Console.WriteLine();
            Console.WriteLine("Calling UsersService to add users.");
            User[] toCreateList = (User[])usersToAdd.ToArray(typeof(User));
            createdUsers = UsersService.CreateUsers(toCreateList);

            Console.WriteLine();

            if (createdUsers == null || createdUsers.Length == 0)
            {

                int addedCount = (createdUsers == null ? 0 : createdUsers.Length);

                Console.WriteLine(String.Format("Error Occured During Creating Some Of Users. Number of Created Users:{0}", addedCount));
            }
            else
            {
                Console.WriteLine("Finished Users Creation.");
            }
        }
    }
}

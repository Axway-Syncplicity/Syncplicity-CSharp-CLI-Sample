using System;
using System.Collections.Generic;
using System.Net;
using CSharpSampleApp.Entities;
using CSharpSampleApp.Util;
using CSharpSampleApp.Services;


namespace CSharpSampleApp.Examples
{
    public class ProvisioningExample
    {
        private const int NewUsersCount = 5;

        private static User[] _createdUsers;
        private static User[] _groupMemberUsers;
        private static Group _createdGroup;

        /*
         * Provisioning
         * - Creating new users associated with a company
         * - Creating a new user group (a group as defined in Syncplicity as having access to the same shared folders)
         * - Associating the newly created users with the new user group
         * - Removing the users from the group
         * - Deleting the group
         */
        public static void Execute()
        {
            GetCompany();
            CreateUsers();
            CreateGroup();
            AddUsersToGroup();
            RemoveUsersFromGroup();
            DeleteGroup();
            DeleteUsers();
        }

        private static void GetCompany()
        {
            Console.WriteLine();
            Console.WriteLine("Get Company started...");
            CompanyService.GetCompany();
        }

        private static void CreateUsers()
        {
            var baseEmail = GenerateRandomBaseEmail();

            // Create users with email typed by user + sequence number
            Console.WriteLine();
            Console.WriteLine("Start Users Creation...");

            var usersToAdd = GenerateUsers(baseEmail);

            Console.WriteLine();
            Console.WriteLine("Calling UsersService to add users.");
            _createdUsers = UsersService.CreateUsers(usersToAdd.ToArray());

            Console.WriteLine();

            if (_createdUsers == null || _createdUsers.Length == 0)
            {

                var addedCount = _createdUsers?.Length ?? 0;

                Console.WriteLine($"Error Occurred During Creating Some Of Users. Number of Created Users: {addedCount}");
            }
            else
            {
                Console.WriteLine("Finished Users Creation.");
            }
        }

        private static void CreateGroup()
        {
            Console.WriteLine();

            if (!ValidateContextPreGroupCreation()) return;

            Console.WriteLine("Start Group Creation...");

            var group = GenerateGroup();

            var createdGroups = GroupsService.CreateGroups(
                                        ApiContext.CompanyGuid.Value,
                                        new[] { group }
                                    );

            Console.WriteLine();

            if (createdGroups == null || createdGroups.Length == 0)
            {
                Console.WriteLine("Error Occurred During Creating Group.");
                return;
            }

            _createdGroup = createdGroups[0];

            Console.WriteLine($"Finished Group Creation. New Group Id: {_createdGroup.Id}");
        }

        private static void AddUsersToGroup()
        {

            if (_createdUsers == null || _createdUsers.Length == 0 || _createdGroup == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Start Adding Group Members...");

            _groupMemberUsers = GroupMembersService.AddGroupMembers(
                                  _createdGroup.Id,
                                  _createdUsers
                               );

            Console.WriteLine();

            if (_groupMemberUsers == null || _groupMemberUsers.Length == 0)
            {

                var addedCount = _groupMemberUsers?.Length ?? 0;
                Console.WriteLine($"Error Occurred During Adding Some Of Members. Number of Added Members: {addedCount}");

                return;
            }

            Console.WriteLine("Finish Adding Group Members.");
        }

        private static void RemoveUsersFromGroup()
        {
            if (_groupMemberUsers == null || _groupMemberUsers.Length == 0 || _createdGroup == null)
            {
                return;
            }

            Console.WriteLine();

            // Remove selected group member
            Console.WriteLine();
            Console.WriteLine("Start Group Member Deletion...");

            foreach (var user in _groupMemberUsers)
            {
                Console.WriteLine();
                Console.WriteLine($"\tRemoving user {user.EmailAddress} from group {_createdGroup.Id}.");
                GroupMembersService.DeleteGroupMember(
                        _createdGroup.Id,
                        user.EmailAddress);
            }

            Console.WriteLine();
            Console.WriteLine("Finish Group Member Deletion.");
        }

        private static void DeleteGroup()
        {
            if (_createdGroup == null)
            {
                return;
            }

            Console.WriteLine();

            // Remove the previously created group
            Console.WriteLine();
            Console.WriteLine($"Delete Group With Id {_createdGroup.Id}");
            GroupsService.DeleteGroup(_createdGroup.Id);
        }

        private static void DeleteUsers()
        {
            if (_createdUsers == null)
            {
                return;
            }

            Console.WriteLine();

            // Remove created users
            Console.WriteLine();
            Console.WriteLine("Start Users Deletion...");

            foreach (var user in _createdUsers)
            {
                Console.WriteLine($"\tDelete User With Email {user.EmailAddress}");
                UsersService.DeleteUser(user.EmailAddress);
            }

            Console.WriteLine();
            Console.WriteLine("Finished Users Deletion.");
        }

        private static bool ValidateContextPreGroupCreation()
        {
            if (ApiContext.CompanyGuid != null) return true;

            Console.WriteLine("Group Can't Be Created Because Company Guid Wasn't Received During Authorization.");
            return false;

        }

        private static Group GenerateGroup()
        {
            var random = new Random();

            var group = new Group
            {
                // Use timestamp to generate unique name
                Name = ConfigurationHelper.GroupName + random.Next(),

                Owner = new User { EmailAddress = ConfigurationHelper.OwnerEmail }
            };
            return group;
        }

        private static string GenerateRandomBaseEmail()
        {
            var random = new Random();
            var baseEmail = $"fake-user-{random.Next().ToString().Substring(0, 7)}@dispostable.com";
            return baseEmail;
        }

        private static List<User> GenerateUsers(string baseEmail)
        {
            var emailParts = baseEmail.Split('@');

            var usersToAdd = new List<User>();
            for (var i = 1; i <= NewUsersCount; i++)
            {
                var user = CreateUserObject(emailParts, i);
                usersToAdd.Add(user);

                EnsureUserDoesNotExist(i, user.EmailAddress);
            }

            return usersToAdd;
        }

        private static User CreateUserObject(IReadOnlyList<string> emailParts, int i)
        {
            var user = new User
            {
                AccountType = AccountType.LimitedBusiness,
                EmailAddress = $"{emailParts[0]}-{i}@{emailParts[1]}",
                Password = ConfigurationHelper.SimplePassword,
                FirstName = $"{emailParts[0]}-{i}",
                LastName = $"Lastname {i}"
            };
            return user;
        }

        private static void EnsureUserDoesNotExist(int userIndex, string userEmailAddress)
        {
            Console.WriteLine();
            Console.WriteLine($"\tPreparing User #{userIndex} [{userEmailAddress}].");
            Console.WriteLine($"\tChecking if user [{userEmailAddress}] already exists.");

            Console.WriteLine();

            var checkUser = TryGetUserByEmail(userEmailAddress);
            if (checkUser != null)
            {
                // If this is the second time running, we'll need to clean up (delete) previous run's users
                // This is just to keep the sample code working as if it was the first time run...
                Console.WriteLine(
                    $"\tCleanup of User #{userIndex} [{userEmailAddress}].  Deleting user that may have been created from previous Sample App run.");
                try
                {
                    UsersService.DeleteUser(userEmailAddress);
                }
                catch (Exception)
                {
                    // Ignore exceptions as this user may not exist
                }
            }
            else
            {
                Console.WriteLine("\tNo user found in company, continuing successfully.");
            }
        }

        private static User TryGetUserByEmail(string userEmailAddress)
        {
            try
            {
                return UsersService.GetUser(userEmailAddress);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)e.Response;
                    var permissionDenied = response.StatusCode == HttpStatusCode.Forbidden &&
                                           response.StatusDescription == "Not A Business Admin";
                    if (permissionDenied)
                    {
                        // For some reason, API sandbox account seems to not have permissions to get company users, and the request is likely to fail.
                        // Skip it and assume we don't have intersection in user names thanks to using random numbers in name generation.
                        return null;
                    }
                }

                throw;
            }
        }
    }
}

using System;

using CSharpSampleApp.Entities;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to group_members.svc and group_member.svc
    /// </summary>
    public class GroupMembersService : APIGateway
    {       
        #region Properties
        /// <summary>
        /// Gets or sets url to GroupMembers service.
        /// </summary>
        protected static string GroupMembersUrl 
        {
            get { return ProvisioningAPIUrlPrefix + "group_members.svc/{0}"; }
        }

        /// <summary>
        /// Gets or sets url to GroupMember service.
        /// </summary>
        protected static string GroupMemberUrl 
        {
            get { return ProvisioningAPIUrlPrefix + "group_member.svc/{0}/member/{1}"; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds users to group.
        /// </summary>
        /// <param name="groupGuid">Group Guid.</param>
        /// <param name="users">Array of users.</param>
        /// <returns>Array of added users.</returns>
        public static User[] AddGroupMembers(Guid groupGuid, User[] users)
        {
            return HttpPost(string.Format(GroupMembersUrl, groupGuid), users);
        }

        /// <summary>
        /// Gets group members by group Guid.
        /// </summary>
        /// <param name="groupGuid">Group Guid.</param>
        /// <returns>Array of users which are group members.</returns>
        public static User[] GetGroupMembers(Guid groupGuid)
        {
            return HttpGet<User[]>(string.Format(GroupMembersUrl, groupGuid));
        }

        /// <summary>
        /// Deletes user from group.
        /// </summary>
        /// <param name="groupGuid">Group Guid.</param>
        /// <param name="userEmail">Email of deleted group member.</param>
        public static void DeleteGroupMember(Guid groupGuid, string userEmail)
        {
            HttpDelete<Group>(string.Format(GroupMemberUrl, groupGuid, userEmail));
        }

        #endregion Methods
    }
}

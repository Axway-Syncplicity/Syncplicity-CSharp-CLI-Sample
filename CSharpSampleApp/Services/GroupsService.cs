using System;

using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to group.svc and groups.svc
    /// </summary>
    public class GroupsService: ApiGateway
    {
        #region Variables

        /// <summary>
        /// Gets or sets url to GroupMembers service.
        /// </summary>
        protected static string GroupsUrl => ProvisioningAPIUrlPrefix + "groups.svc/{0}/groups";

        /// <summary>
        /// Gets or sets url to GroupMember service.
        /// </summary>
        protected static string GroupUrl => ProvisioningAPIUrlPrefix + "group.svc/{0}";

        #endregion Variables

        #region Methods

        /// <summary>
        /// Creates new groups in company.
        /// </summary>
        /// <param name="companyGuid">Company Guid.</param>
        /// <param name="groups">Array of groups to be created.</param>
        /// <returns>Array of created groups.</returns>
        public static Group[] CreateGroups(Guid companyGuid, Group[] groups)
        {
            return HttpPost(string.Format(GroupsUrl, companyGuid), groups);
        }

        /// <summary>
        /// Gets groups of company by Company Guid.
        /// </summary>
        /// <param name="companyGuid">Company Guid.</param>
        /// <param name="include">Include parameter.</param>
        /// <returns>Array of company groups.</returns>
        public static Group[] GetGroups(Guid companyGuid, string include = null)
        {
            var uri = string.Format(GroupsUrl, companyGuid);
            if (!string.IsNullOrEmpty(include))
            {
                uri = $"{uri}?include={include}";
            }

            return HttpGet<Group[]>(uri);
        }

        /// <summary>
        /// Deletes group by Guid.
        /// </summary>
        /// <param name="groupGuid">Group Guid.</param>
        public static void DeleteGroup(Guid groupGuid)
        {
            HttpDelete<Group>(string.Format(GroupUrl, groupGuid));
        }

        #endregion Methods
    }
}

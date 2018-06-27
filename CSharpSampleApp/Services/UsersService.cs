using CSharpSampleApp.Entities;
using CSharpSampleApp.Services;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to users.svc, users_public.svc and user.svc
    /// </summary>
    public class UsersService : APIGateway
    {

        #region Static Members

        /// <summary>
        /// Gets url to GroupMembers service.
        /// </summary>
        protected static string UsersUrl
        {
            get { return ProvisioningAPIUrlPrefix + "users.svc/"; }
        }

        /// <summary>
        /// Gets url to GroupMembers service.
        /// </summary>
        protected static string UserUrl
        {
            get { return ProvisioningAPIUrlPrefix + "user.svc/{0}"; }
        }

        /// <summary>
        /// Gets url to GroupMember service.
        /// </summary>
        protected static string UsersPublicUrl
        {
            get { return ProvisioningAPIUrlPrefix + "users_public.svc/"; }
        }

        #endregion Static Members

        #region Methods

        /// <summary>
        /// Retrieve a user from the company, if it exists.
        /// </summary>
        /// <param name="users">Email of the user.</param>
        /// <returns>Populated User object of the related user.</returns>
        public static User GetUser(string email)
        {
            return HttpGet<User>(string.Format(UserUrl, email));
        }


        /// <summary>
        /// Creates new users for current company.
        /// </summary>
        /// <param name="users">Array of users to be created.</param>
        /// <returns>Array of created users.</returns>
        public static User[] CreateUsers(User[] users)
        {
            return HttpPost(UsersUrl, users);
        }

        /// <summary>
        /// Deletes user by email.
        /// </summary>
        /// <param name="email">Email of deleted user.</param>
        public static void DeleteUser(string email)
        {
            HttpDelete<User>(string.Format(UserUrl, email));
        }

        #endregion Methods
    }
}

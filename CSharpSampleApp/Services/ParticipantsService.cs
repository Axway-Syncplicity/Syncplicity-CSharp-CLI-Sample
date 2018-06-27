using System;
using System.Linq;
using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to syncpoint_participants.svc and syncpoint_participants.svc
    /// </summary>
    public class ParticipantsService : APIGateway
    {
        #region Variables

        private static string _ParticipantsUrl = SyncpointAPIUrlPrefix + "syncpoint_participants.svc/{0}/participants";

        #endregion Variables

        #region Methods

        public static void RemoveParticipants(long syncpointId, params string[] emails)
        {
            var participants = emails.Select(x => new Participant() {User = new User() {EmailAddress = x}}).ToArray();
            HttpDelete<string>(String.Format(_ParticipantsUrl, syncpointId), participants);
        }

        #endregion Methods
    }
}
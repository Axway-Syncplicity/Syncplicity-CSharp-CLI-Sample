using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpSampleApp.Entities;
using CSharpSampleApp.Services;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Examples
{
    public static class CrossRolExample
    {
        public static void Execute()
        {
            var syncpoint = CreateSyncpoint();
			ShareSyncpoint(syncpoint);
			syncpoint = GetSyncpoint(syncpoint.Id);
			var currentRol = GolGateway.CurrentRol;
			foreach (var participant in syncpoint.Participants)
			{
                // if there is no participants email - this means that he belongs to another ROL, lets get his pii info
                if (participant.User.OriginalRolId != null && participant.User.OriginalRolId.Value != currentRol.Id)
				{
                    //GolGateway.SwitchRol(participant.User.OriginalRolId.Value);

					var homePii = UserPIIService.GetUsersPiis(new[] { participant.User.Id });
					if (homePii != null && homePii.RolUserPIIs != null)
					{
                        GolGateway.SwitchRol(participant.User.OriginalRolId.Value);
                        homePii = UserPIIService.GetUsersPiis(new[] { participant.User.Id }, homePii.RolUserPIIs[0].Bearer);

                        Console.WriteLine(@"Received User PIIs from Rol with Id = {GolGateway.CurrentRol.Id}");
						foreach (var user in homePii.RolUserPIIs.SelectMany(x => x.Users))
						{
							Console.WriteLine(@"Received User PII Email {user.EmailAddress} for user {user.Id} from Rol {GolGateway.CurrentRol.Id}");
						}
					}
				}
			}
			GolGateway.SwitchRol(currentRol.Id);
			OAuth.OAuth.authenticate();
			DeleteSyncpoint(syncpoint.Id);
        }

        private static void ShareSyncpoint(SyncPoint syncpoint)
        {
            SyncPointsService.PostSyncPointParticipants(syncpoint.Id, new []
            {
                new Participant {
					User = new User
					{
						EmailAddress = ConfigurationHelper.OtherRolEmail,
					},
					Permission = SharingPermission.ReadWrite
				}
            });
        }

        private static SyncPoint CreateSyncpoint()
        {
            return SyncPointsService.CreateSyncpoints(new[]
            {
                new SyncPoint
                {
                    Name = string.Format("syncpoint-{0}", new Random().Next()),
                    Type = SyncPointType.Custom
                }
            }).First();
        }

		private static SyncPoint GetSyncpoint(long syncpointId)
		{
			return SyncPointsService.GetSyncpoint(syncpointId, SyncPointsService.Include.Participants | SyncPointsService.Include.Inviter);
		}

		private static void DeleteSyncpoint(long syncpointId)
		{
			SyncPointsService.DeleteSyncpoint(syncpointId);
		}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpSampleApp.Entities;
using CSharpSampleApp.Services;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Examples
{
    public static class CrossRolExample
    {
        public static void Execute()
        {
            var isConfigValid = ValidateConfig();
            if (!isConfigValid) return;

            var syncpoint = CreateSyncpoint();
			ShareSyncpoint(syncpoint);
			syncpoint = GetSyncpoint(syncpoint.Id);
			var currentRol = GolGateway.CurrentRol;

            if (syncpoint.Participants.Any())
            {
                foreach (var participant in syncpoint.Participants)
                {
                    // if there is no participants email - this means that he belongs to another ROL, lets get his pii info
                    if (participant.User.OriginalRolId == null ||
                        participant.User.OriginalRolId == currentRol.Id) continue;

                    //GolGateway.SwitchRol(participant.User.OriginalRolId.Value);

                    var homePii = UserPiiService.GetUsersPiis(new[] {participant.User.Id});
                    if (homePii?.RolUserPIIs == null) continue;

                    GolGateway.SwitchRol(participant.User.OriginalRolId.Value);
                    homePii = UserPiiService.GetUsersPiis(new[] {participant.User.Id}, homePii.RolUserPIIs[0].Bearer);

                    Console.WriteLine($"Received User PIIs from Rol with Id = {GolGateway.CurrentRol.Id}");
                    foreach (var user in homePii.RolUserPIIs.SelectMany(x => x.Users))
                    {
                        Console.WriteLine(
                            $"Received User PII Email {user.EmailAddress} for user {user.Id} from Rol {GolGateway.CurrentRol.Id}");
                    }
                }

                GolGateway.SwitchRol(currentRol.Id);
                OAuth.OAuth.Authenticate();
            }
            else
            {
                Console.WriteLine("Error: expected at least one participant added to syncpoint, but received none.");
            }

            DeleteSyncpoint(syncpoint.Id);
        }

        private static bool ValidateConfig()
        {
            var errors = EvaluateValidationRules().ToList();
            if (!errors.Any()) return true;

            Console.WriteLine("Error: Cannot proceed to cross-ROL sample because of configuration errors:");
            errors.ForEach(e => Console.WriteLine($"\tError: {e}"));
            return false;
        }

        private static IEnumerable<string> EvaluateValidationRules()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationHelper.OtherRolEmail))
                yield return "otherRolEmail is not specified";
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
                    Name = $"syncpoint-{new Random().Next()}",
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

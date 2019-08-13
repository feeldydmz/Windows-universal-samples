using System.Linq;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;
using Newtonsoft.Json;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    internal static class ClinetProfilesExtension
    {
        public static ClientProfile GetProfile(this ClientProfiles clientProfiles, string profileId)
        {
            return clientProfiles.Profiles?.FirstOrDefault(s => s.Id == profileId);
        }

        public static ClientProfile GetSelectedProfile(this ClientProfiles clientProfiles)
        {
            return clientProfiles.Profiles?.FirstOrDefault(s => s.Id == clientProfiles.SelectedProfileId);
        }

        public static string GetJsonString(this ClientProfiles clientProfiles)
        {
            return JsonConvert.SerializeObject(clientProfiles);
        }

        public static ClientProfilesModel ToPersistentModel(this ClientProfiles clientProfiles, bool shouldEncrypt)
        {
            return new ClientProfilesModel(clientProfiles.ProgramKey,
                clientProfiles.SelectedProfileId,
                clientProfiles.Profiles?.Select(p => p.ToPersistentModel(shouldEncrypt))
                    .ToList(),
                clientProfiles.ProfileRegions?.Select(r => r.ToPersistentModel(shouldEncrypt))
                    .ToList());
        }
    }
}
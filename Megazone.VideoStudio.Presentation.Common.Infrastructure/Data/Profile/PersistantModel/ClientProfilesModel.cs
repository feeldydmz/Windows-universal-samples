using System.Collections.Generic;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel
{
    public class ClientProfilesModel
    {
        public ClientProfilesModel(string programKey,
            string selectedProfileId,
            IEnumerable<ClientProfileModel> profiles,
            IEnumerable<ProfileRegionModel> profileRegions)
        {
            ProgramKey = programKey;
            SelectedProfileId = selectedProfileId;
            Profiles = profiles;
            ProfileRegions = profileRegions;
        }

        public string ProgramKey { get; }
        public string SelectedProfileId { get; }
        public IEnumerable<ClientProfileModel> Profiles { get; }
        public IEnumerable<ProfileRegionModel> ProfileRegions { get; set; }
    }
}
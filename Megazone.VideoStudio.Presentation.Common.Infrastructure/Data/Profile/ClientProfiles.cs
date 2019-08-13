using System.Collections.Generic;
using System.Linq;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    /// <summary>
    ///     JSON Mapping용
    /// </summary>
    public class ClientProfiles
    {
        public ClientProfiles(IEnumerable<ClientProfile> profiles)
        {
            Profiles = profiles;
        }

        public ClientProfiles(ClientProfilesModel data)
        {
            ProgramKey = data.ProgramKey;
            SelectedProfileId = data.SelectedProfileId;
            Profiles = data.Profiles?.Select(p => new ClientProfile(p))
                           .ToList() ??
                       new List<ClientProfile>();
            ProfileRegions = data.ProfileRegions?.Select(r => new ProfileRegion(r))
                                 .ToList() ??
                             new List<ProfileRegion>();
        }

        public ClientProfiles(string programKey)
        {
            ProgramKey = programKey;
            Profiles = new List<ClientProfile>();
        }

        public string ProgramKey { get; set; }

        public string SelectedProfileId { get; set; }

        public IEnumerable<ClientProfile> Profiles { get; set; }

        public IEnumerable<ProfileRegion> ProfileRegions { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    public class ProfileRegion
    {
        public ProfileRegion(string profileID)
        {
            ProfileID = profileID;
        }

        public ProfileRegion(ProfileRegionModel data)
        {
            ProfileID = data.ProfileID;
            RegionInformations = data.RegionInformations?.Select(r => new RegionInformation(r))
                                     .ToList() ??
                                 new List<RegionInformation>();
        }

        public string ProfileID { get; }

        public IEnumerable<RegionInformation> RegionInformations { get; set; } = new List<RegionInformation>();
    }
}
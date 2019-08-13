using System.Linq;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    internal static class ProfileRegionExtension
    {
        public static ProfileRegionModel ToPersistentModel(this ProfileRegion profileRegion, bool shouldEncrypt)
        {
            return new ProfileRegionModel(profileRegion.ProfileID,
                profileRegion.RegionInformations?.Select(r => r.ToPersistantModel(shouldEncrypt))
                    .ToList());
        }
    }
}
using System.Collections.Generic;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel
{
    public class ProfileRegionModel
    {
        public ProfileRegionModel(string profileId, IEnumerable<RegionInformationModel> regionInformations)
        {
            ProfileID = profileId;
            RegionInformations = regionInformations;
        }

        public string ProfileID { get; }

        public IEnumerable<RegionInformationModel> RegionInformations { get; }
    }
}
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    internal static class RegionInformationExtension
    {
        public static RegionInformationModel ToPersistantModel(this RegionInformation regionInformation,
            bool shouldEncrypt)
        {
            return new RegionInformationModel(regionInformation.Name, regionInformation.Code, regionInformation.API,
                regionInformation.IsDefault);
        }
    }
}
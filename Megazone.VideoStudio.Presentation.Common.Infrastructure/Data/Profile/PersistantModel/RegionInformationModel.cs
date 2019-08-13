namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel
{
    public class RegionInformationModel
    {
        public RegionInformationModel(string name, string code, string api, bool isDefault)
        {
            Name = name;
            Code = code;
            API = api;
            IsDefault = isDefault;
        }

        public string Code { get; }

        public string Name { get; }

        public string API { get; }

        public bool IsDefault { get; }
    }
}
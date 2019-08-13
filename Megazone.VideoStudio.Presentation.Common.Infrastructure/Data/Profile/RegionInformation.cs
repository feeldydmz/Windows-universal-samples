using Megazone.Core.Windows.Mvvm;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    public class RegionInformation : BindableBase
    {
        private string _api;
        private string _code;
        private bool _isDefault;
        private string _name;

        public RegionInformation()
        {
        }

        public RegionInformation(RegionInformationModel data)
        {
            _api = data.API;
            _code = data.Code;
            _isDefault = data.IsDefault;
            _name = data.Name;
        }

        public string Code
        {
            get => _code;
            set => Set(ref _code, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string API
        {
            get => _api;
            set => Set(ref _api, value);
        }

        public bool IsDefault
        {
            get => _isDefault;
            set => Set(ref _isDefault, value);
        }

        public RegionInformationModel ToPersistantModel(bool shouldEncrypt)
        {
            return new RegionInformationModel(Name, Code, API, IsDefault);
        }
    }
}
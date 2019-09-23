using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class LanguageItemViewModel : ViewModelBase
    {
        private string _languageCode;
        private string _countryCode;
        private string _countryName;

        public string LanguageCode
        {
            get => _languageCode;

            set => Set(ref _languageCode, value);
        }

        public string CountryCode
        {
            get => _countryCode;

            set => Set(ref _countryCode, value);
        }

        public string CountryName
        {
            get => _countryName;

            set => Set(ref _countryName, value);
        }

        public string DisplayName => CountryName + " ( " + LanguageCode + "-" + CountryCode + " )";
    }
}
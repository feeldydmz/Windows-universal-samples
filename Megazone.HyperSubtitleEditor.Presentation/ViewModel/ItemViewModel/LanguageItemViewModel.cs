//using Megazone.Cloud.Media.ServiceInterface.Model;
//using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

//namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
//{
//    internal class LanguageItemViewModel : ViewModelBase
//    {
//        private string _countryCode;
//        private string _countryName;

//        private string _languageCode;
//        private string _languageName;

//        public LanguageItemViewModel(Language source)
//        {
//            Source = source;
//            LanguageCode = source?.LanguageCode;
//            LanguageName = source?.LanguageName;
//            CountryCode = source?.CountryCode;
//            CountryName = source?.CountryName;
//        }

//        public Language Source { get; }

//        public string LanguageCode
//        {
//            get => _languageCode;
//            set => Set(ref _languageCode, value);
//        }

//        public string LanguageName
//        {
//            get => _languageName;
//            set => Set(ref _languageName, value);
//        }

//        public string CountryCode
//        {
//            get => _countryCode;
//            set => Set(ref _countryCode, value);
//        }

//        public string CountryName
//        {
//            get => _countryName;
//            set => Set(ref _countryName, value);
//        }
//    }
//}


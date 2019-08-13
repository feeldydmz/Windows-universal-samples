using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class LanguageItemViewModel : ViewModelBase
    {
        private string _languageCode;
        private string _nativeName;

        public string LanguageCode
        {
            get => _languageCode;

            set => Set(ref _languageCode, value);
        }

        public string NativeName
        {
            get => _nativeName;

            set => Set(ref _nativeName, value);
        }

        public string DisplayName => NativeName + " ( " + LanguageCode + " )";
    }
}
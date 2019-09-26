using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Language;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class ImportExcelItemViewModel : ViewModelBase
    {
        private readonly LanguageLoader _languageLoader;
        private bool _isChecked;
        private string _label;
        private IList<LanguageItemViewModel> _languages;
        private LanguageItemViewModel _selectedLanguageItemViewModel;
        private CaptionKind _selectedSubtitleKind;
        private string _sheetName;
        private IList<CaptionKind> _subtitleKinds;

        public ImportExcelItemViewModel(LanguageLoader languageLoader)
        {
            _languageLoader = languageLoader;

            _subtitleKinds = new List<CaptionKind>
            {
                CaptionKind.Subtitle,
                CaptionKind.Caption,
                CaptionKind.Chapter
            };

            _languages = new List<LanguageItemViewModel>();

            var languages = _languageLoader.Languages;

            foreach (var item in languages.ToList())
                Languages.Add(new LanguageItemViewModel
                {
                    LanguageCode = item.Alpha2,
                    CountryCode = item.CountryInfo.Alpha2,
                    CountryName = item.CountryInfo.Name
                });
        }

        public string Label
        {
            get => _label;

            set => Set(ref _label, value);
        }

        public IList<LanguageItemViewModel> Languages
        {
            get => _languages;

            set => Set(ref _languages, value);
        }

        public LanguageItemViewModel SelectedLanguageItemViewModel
        {
            get => _selectedLanguageItemViewModel;

            set
            {
                var languageCode = value.LanguageCode.ToLower();
                var countryCode = value.CountryCode.ToUpper();

                foreach (var item in Languages.ToList())
                    if (item.LanguageCode.Equals(languageCode) && item.CountryCode.Equals(countryCode))
                        Set(ref _selectedLanguageItemViewModel, item);
            }
        }

        public IList<CaptionKind> SubtitleKinds
        {
            get => _subtitleKinds;

            set => Set(ref _subtitleKinds, value);
        }

        public CaptionKind SelectedSubtitleKind
        {
            get => _selectedSubtitleKind;

            set => Set(ref _selectedSubtitleKind, value);
        }

        public bool IsChecked
        {
            get => _isChecked;

            set => Set(ref _isChecked, value);
        }

        public string SheetName
        {
            get => _sheetName;

            set => Set(ref _sheetName, value);
        }
    }
}
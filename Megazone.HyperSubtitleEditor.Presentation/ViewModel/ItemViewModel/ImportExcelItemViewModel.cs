using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class ImportExcelItemViewModel : ViewModelBase
    {
        private bool _isChecked;
        private string _label;
        private IList<LanguageItem> _languages;
        private LanguageItem _selectedLanguage;
        private CaptionKind _selectedSubtitleKind;
        private string _sheetName;

        public ImportExcelItemViewModel(LanguageLoader languageLoader)
        {
            Languages = languageLoader.Languages?.ToList();
        }

        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        }

        public IList<LanguageItem> Languages
        {
            get => _languages;
            set => Set(ref _languages, value);
        }

        public LanguageItem SelectedLanguage
        {
            get => _selectedLanguage;
            set => Set(ref _selectedLanguage, value);
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
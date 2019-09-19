﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Language;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class ImportExcelItemViewModel : ViewModelBase
    {
        private bool _isChecked;
        private string _label;
        private IList<LanguageItemViewModel> _languages;
        private LanguageItemViewModel _selectedLanguageItemViewModel;
        private CaptionKind _selectedSubtitleKind;
        private string _sheetName;
        private IList<CaptionKind> _subtitleKinds;

        public ImportExcelItemViewModel()
        {
            _subtitleKinds = new List<CaptionKind>
            {
                CaptionKind.Subtitle,
                CaptionKind.Caption,
                CaptionKind.Chapter
            };
            _languages = new List<LanguageItemViewModel>();
            var preferedLanguageInfoFilePath = Path.GetDirectoryName(
                                                   Assembly.GetExecutingAssembly()
                                                       .Location) +
                                               "\\PreferedLanguageInfo.json";
            foreach (var item in LanguageParser.GetLanguages(preferedLanguageInfoFilePath)
                .ToList())
                Languages.Add(new LanguageItemViewModel
                {
                    LanguageCode = item.Alpha2,
                    NativeName = item.NativeName
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
                var code = value.LanguageCode.ToLower();

                foreach (var item in Languages.ToList())
                    if (item.LanguageCode.Equals(code))
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Language;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal abstract class CreateSubtitleViewModelBase : ViewModelBase
    {
        private readonly LanguageParser _languageParser;
        protected readonly ILogger Logger;
        private string _label;
        private IList<LanguageItemViewModel> _languages;
        private ICommand _onConfirmCommand;
        private LanguageItemViewModel _selectedLanguageItemViewModel;
        private CaptionKind _selectedSubtitleKind;
        private IList<CaptionKind> _subtitleKinds;

        protected CreateSubtitleViewModelBase(ILogger logger, LanguageParser languageParser)
        {
            Logger = logger;
            _languageParser = languageParser;

            _subtitleKinds = new List<CaptionKind>
            {
                CaptionKind.Subtitle,
                CaptionKind.Caption,
                CaptionKind.Chapter
            };
            _languages = new List<LanguageItemViewModel>();

            var languages = _languageParser.Languages;

            foreach (var item in languages.ToList())
                Languages.Add(new LanguageItemViewModel
                {
                    LanguageCode = item.Alpha2,
                    CountryCode = item.CountryInfo.Alpha2,
                    CountryName = item.CountryInfo.Name
                });

            var result = Languages.Where(i => i.LanguageCode.Equals("en"));

            SelectedLanguageItemViewModel = result.ToList().FirstOrDefault();
            SelectedSubtitleKind = CaptionKind.Subtitle;
        }

        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        }

        public Action CloseAction { protected get; set; }

        public IList<LanguageItemViewModel> Languages
        {
            get => _languages;
            set => Set(ref _languages, value);
        }

        public LanguageItemViewModel SelectedLanguageItemViewModel
        {
            get => _selectedLanguageItemViewModel;
            set => Set(ref _selectedLanguageItemViewModel, value);
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

        public ICommand OnConfirmCommand
        {
            get { return _onConfirmCommand = _onConfirmCommand ?? new RelayCommand(OnConfirm, CanOnConfirm); }
        }

        protected abstract bool CanOnConfirm();
        protected abstract void OnConfirm();
    }
}
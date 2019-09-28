using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal abstract class CreateSubtitleViewModelBase : ViewModelBase
    {
        protected readonly ILogger Logger;
        private string _label;
        private IList<Language> _languages;
        private ICommand _onConfirmCommand;
        private Language _selectedLanguage;
        private CaptionKind _selectedSubtitleKind;
        private IList<CaptionKind> _subtitleKinds;

        protected CreateSubtitleViewModelBase(ILogger logger, LanguageLoader languageLoader)
        {
            Logger = logger;

            _subtitleKinds = new List<CaptionKind>
            {
                CaptionKind.Subtitle,
                CaptionKind.Caption,
                CaptionKind.Chapter
            };
            
            SelectedSubtitleKind = CaptionKind.Subtitle;

            Languages = languageLoader.Languages?.ToList();
            SelectedLanguage = Languages
                ?.Where(language => language.LanguageCode?.ToLower().Equals("en") ?? false).FirstOrDefault();
        }

        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        }

        public Action CloseAction { protected get; set; }

        public IList<Language> Languages
        {
            get => _languages;
            set => Set(ref _languages, value);
        }

        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set => Set(ref _selectedLanguage, value);
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
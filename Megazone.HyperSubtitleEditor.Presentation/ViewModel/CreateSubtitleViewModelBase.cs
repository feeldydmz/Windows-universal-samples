using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal abstract class CreateSubtitleViewModelBase : ViewModelBase
    {
        protected readonly ILogger Logger;
        private string _label;
        private IList<LanguageItem> _languages;
        private ICommand _onConfirmCommand;
        private LanguageItem _selectedLanguage;
        private CaptionKind _selectedSubtitleKind;

        protected CreateSubtitleViewModelBase(ILogger logger, LanguageLoader languageLoader)
        {
            Logger = logger;

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

        public ICommand OnConfirmCommand
        {
            get { return _onConfirmCommand = _onConfirmCommand ?? new RelayCommand(OnConfirm, CanOnConfirm); }
        }

        protected abstract bool CanOnConfirm();
        protected abstract void OnConfirm();
    }
}
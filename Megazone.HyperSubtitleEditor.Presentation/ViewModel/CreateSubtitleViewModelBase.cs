using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Megazone.Api.Transcoder.Domain;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Language;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal abstract class CreateSubtitleViewModelBase : ViewModelBase
    {
        protected readonly ILogger Logger;
        private string _label;
        private IList<LanguageItemViewModel> _languages;
        private ICommand _onConfirmCommand;
        private LanguageItemViewModel _selectedLanguageItemViewModel;
        private TrackKind _selectedSubtitleKind;
        private IList<TrackKind> _subtitleKinds;

        protected CreateSubtitleViewModelBase(ILogger logger)
        {
            Logger = logger;
            _subtitleKinds = new List<TrackKind>
            {
                TrackKind.Subtitle,
                TrackKind.Caption,
                TrackKind.Chapter
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
            SelectedLanguageItemViewModel = Languages.Where(i => i.LanguageCode.Equals("en"))
                .ToList()
                .FirstOrDefault();
            SelectedSubtitleKind = TrackKind.Subtitle;
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

        public IList<TrackKind> SubtitleKinds
        {
            get => _subtitleKinds;
            set => Set(ref _subtitleKinds, value);
        }

        public TrackKind SelectedSubtitleKind
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
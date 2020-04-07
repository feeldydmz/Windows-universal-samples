using System;
using System.IO;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.VideoTrack;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class OpenSubtitleViewModel : CreateSubtitleViewModelBase
    {
        private readonly FileManager _fileManager;
        private string _filePath;
        private SubtitleFormatKind _subtitleFormat;
        private ICommand _loadedCommand;
        private ICommand _openSubtitleFileCommand;

        public OpenSubtitleViewModel(ILogger logger, FileManager fileManager, LanguageLoader languageLoader) : base(
            logger, languageLoader)
        {
            _fileManager = fileManager;
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        public SubtitleFormatKind SubtitleFormat
        {
            get => _subtitleFormat;
            set => Set( ref _subtitleFormat, value);
        }

        public ICommand OpenSubtitleFileCommand
        {
            get { return _openSubtitleFileCommand = _openSubtitleFileCommand ?? new RelayCommand(OpenSubtitleFile); }
        }

        //public string InitialFilePath { get; internal set; }

        private void OnLoaded()
        {
            //FilePath = InitialFilePath; 
            Open(FilePath);
        }

        protected override bool CanOnConfirm()
        {
            return !string.IsNullOrEmpty(FilePath) &&
                   !string.IsNullOrEmpty(Label) &&
                   !string.IsNullOrEmpty(SelectedLanguage?.LanguageCode);
        }

        protected override void OnConfirm()
        {
            try
            {
                MessageCenter.Instance.Send(
                    new Message.SubtitleEditor.FileOpenedMessage(this, new FileOpenedMessageParameter
                    {
                        FilePath = FilePath,
                        Kind = SelectedSubtitleKind,
                        Label = Label,
                        LanguageCode = SelectedLanguage.LanguageCode,
                        CountryCode = SelectedLanguage.CountryCode,
                        SubtitleFormat = _subtitleFormat
                    }));
            }
            catch (Exception ex)
            {
                Logger.Error.Write(ex);
            }

            CloseAction?.Invoke();
        }

        private void OpenSubtitleFile()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                string initialPath = ConfigHolder.Current.General.RecentlySubtitleOpenPath;
                FilePath = _fileManager.OpenFile(
                    "subtitle files (*.vtt;*.srt;*.smi)|*.vtt;*.srt;*.smi",
                    initialPath);

                if (!string.IsNullOrEmpty(FilePath))
                    ConfigHolder.Current.General.RecentlySubtitleOpenPath = Path.GetDirectoryName(FilePath);

                SubtitleFormat = SubtitleViewModel.GetSubTitleFormatKindByFileName(FilePath);
            }

            Open(FilePath);
        }

        private void Open(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var container = Path.GetExtension(filePath);
            Label = string.IsNullOrEmpty(container) ? fileName : fileName?.Replace(container, "");
        }
    }
}
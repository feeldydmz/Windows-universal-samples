using System;
using System.IO;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Language;
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
        private ICommand _loadedCommand;
        private ICommand _openSubtitleFileCommand;

        public OpenSubtitleViewModel(ILogger logger, FileManager fileManager, LanguageLoader languageLoader) : base(logger, languageLoader)
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

        public ICommand OpenSubtitleFileCommand
        {
            get { return _openSubtitleFileCommand = _openSubtitleFileCommand ?? new RelayCommand(OpenSubtitleFile); }
        }

        public string InitialFilePath { get; internal set; }

        private void OnLoaded()
        {
            FilePath = InitialFilePath;
            Open(FilePath);
        }

        protected override bool CanOnConfirm()
        {
            return !string.IsNullOrEmpty(FilePath) &&
                   !string.IsNullOrEmpty(Label) &&
                   !string.IsNullOrEmpty(SelectedLanguageItemViewModel?.LanguageCode);
        }

        protected override void OnConfirm()
        {
            try
            {
                var text = File.ReadAllText(FilePath);
                MessageCenter.Instance.Send(
                    new Subtitle.FileOpenedMessage(this, new FileOpenedMessageParameter
                    {
                        FilePath = FilePath,
                        Kind = SelectedSubtitleKind,
                        Label = Label,
                        Text = text,
                        LanguageCode = SelectedLanguageItemViewModel.LanguageCode,
                        CountryCode = SelectedLanguageItemViewModel.CountryCode
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
            FilePath = _fileManager.OpenFile("WebVtt files (*.vtt)|*.vtt");

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
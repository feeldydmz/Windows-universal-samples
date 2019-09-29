using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Serializable;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SaveGroupFileCommand : DependencyObject, ICommand
    {
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        public SaveGroupFileCommand()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab;
        }

        public void Execute(object parameter)
        {
            try
            {
                var subtitleTabItems = SubtitleTabItemParser.Convert(_subtitleViewModel.Tabs);

                var subtitleGroup = new SubtitleGroup(subtitleTabItems,
                    _subtitleViewModel.WorkContext.OpenedVideo?.Id,
                    _subtitleViewModel.WorkContext.OpenedCaptionAsset?.Id,
                    _signInViewModel.SelectedStage.Source,
                    _signInViewModel.SelectedProject.Source);

                var now = DateTime.Now.ToString("yyyyMMddHHmmss");

                var filePath = _fileManager.OpenSaveFileDialog(this.MyDocuments(),
                    "HyperSubtitleEditor files (.hsg)|*.hsg",
                    "HyperSubtitleEditor_Package_" + now + ".hsg");
                BinarySerialization.WriteToBinaryFile(filePath, subtitleGroup);
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Serializable;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension;
using Unity;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SaveGroupFileCommand : DependencyObject, ICommand
    {
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;
        private readonly MainViewModel _mainViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        public SaveGroupFileCommand()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _mainViewModel = Bootstrapper.Container.Resolve<MainViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_mainViewModel.JobId) || _subtitleViewModel.HasTab;
        }

        public void Execute(object parameter)
        {
            try
            {
                var subtitleTabItems = SubtitleTabItemParser.Convert(_subtitleViewModel.Tabs);

                var subtitleGroup = new SubtitleGroup(AppContext.Config.ProfileId, AppContext.Config.PipelineId,
                    _mainViewModel.JobId, AppContext.Config.Region, subtitleTabItems,
                    _subtitleViewModel.WorkContext.OpenedVideo?.Id,
                    _subtitleViewModel.WorkContext.OpenedCaptionAsset?.Id);

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
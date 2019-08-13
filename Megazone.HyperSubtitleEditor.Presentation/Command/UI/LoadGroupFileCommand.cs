using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Serializable;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class LoadGroupFileCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;
        private readonly MainViewModel _mainViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        public LoadGroupFileCommand()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _mainViewModel = Bootstrapper.Container.Resolve<MainViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!string.IsNullOrEmpty(_mainViewModel.JobId) || _subtitleViewModel.HasTab)
            {
                var result = _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                    Resource.MSG_LOAD_GROUP_FILE,
                    MessageBoxButton.OKCancel));

                if (result == MessageBoxResult.Cancel)
                    return;
            }

            try
            {
                var filePath = _fileManager.OpenFile("HyperSubtitleEditor files (.hsg)|*.hsg");

                var group = BinarySerialization.ReadFromBinaryFile<SubtitleGroup>(filePath);

                MessageCenter.Instance.Send(
                    new ReinitializeAppContextMessage(this, group.PipelineId, group.JobId, group.ProfileId,
                        group.Region));

                MessageCenter.Instance.Send(new Subtitle.LoadTabsMessage(this, group.Tabs));
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
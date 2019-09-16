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
        private readonly SignInViewModel _signInViewModel;

        public LoadGroupFileCommand()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _mainViewModel = Bootstrapper.Container.Resolve<MainViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_subtitleViewModel.HasTab)
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
                if (!string.IsNullOrEmpty(group.Stage?.Id) && !string.IsNullOrEmpty(group.Project?.Id))
                {
                    if (!_signInViewModel.SelectedStage.Id.Equals(group.Stage?.Id) ||
                        !_signInViewModel.SelectedProject.ProjectId.Equals(group.Project?.Id))
                    {
                        // [resource]
                        var message =
                            $"'{group.Stage.Name}' 스테이지의 '{group.Project.Name}' 프로젝트에서 오픈할 수 있습니다.프로젝트를 변경한 후 데이터를 불러오십시오.";
                        if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WAITING, message, MessageBoxButton.OK, TextAlignment.Left)) == MessageBoxResult.OK)
                            return;
                    }
                }

                MessageCenter.Instance.Send(new ReinitializeAppContextMessage(this));

                MessageCenter.Instance.Send(new Subtitle.LoadTabsMessage(this, group.Tabs));
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                // [resource]
                var errorMessage = "알 수 없는 오류로 파일 불러오기를 실패 하였습니다. 관리자에게 문의하십시오.";
                _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_ERROR, errorMessage,
                    MessageBoxButton.OK));
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
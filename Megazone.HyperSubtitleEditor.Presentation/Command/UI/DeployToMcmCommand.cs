using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class DeployToMcmCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitleViewModel;

        public DeployToMcmCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }


        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.WorkContext?.CanDeploy() ?? false;
        }

        public async void Execute(object parameter)
        {
            //MessageCenter.Instance.Send(new Subtitle.DeployRequestedMessage(this));

            //TODO: 게시하기 전에 저장이 안되어 있다면, 로컬 저장을 하도록 한다.
            if (_subtitleViewModel.WorkContext.IsModified())
            {
                _browser.ShowConfirmWindow(new ConfirmWindowParameter("경고", "변경된 내용을 저장한 후, 게시 하십시오.",
                    MessageBoxButton.OK));
                return;
            }

            _browser.Main.LoadingManager.Show();
            var uploadInputPath = await _subtitleViewModel.WorkContext.GetUploadInputPathAsync();
            _browser.Main.LoadingManager.Hide();
            _subtitleViewModel.WorkContext.SetUploadInputPath(uploadInputPath);
            if (string.IsNullOrEmpty(uploadInputPath))
            {
                // 메시지 처리.
                // 게시할수 없음.
                _browser.ShowConfirmWindow(
                    new ConfirmWindowParameter("오류", "업로드 설정 정보 조회를 실패하였습니다.\nMedia Cloud 관리자에게 문의하십시오.",
                        MessageBoxButton.OK));
                return;
            }

            _browser.Main.ShowMcmDeployDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
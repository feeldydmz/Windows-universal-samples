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
            return (_subtitleViewModel.WorkContext?.CanDeploy() ?? false) && _subtitleViewModel.HasTab;
        }

        public async void Execute(object parameter)
        {
            _browser.Main.LoadingManager.Show();
            var uploadInputPath = await _subtitleViewModel.WorkContext.GetUploadInputPathAsync();
            _browser.Main.LoadingManager.Hide();
            _subtitleViewModel.WorkContext.SetUploadInputPath(uploadInputPath);
            if (string.IsNullOrEmpty(uploadInputPath))
            {
                // 메시지 처리.
                // 게시할수 없음.
                // [resource]
                _browser.ShowConfirmWindow(
                    new ConfirmWindowParameter("오류", "업로드 설정 정보 조회를 실패하였습니다.\n관리자에게 문의하십시오.",
                        MessageBoxButton.OK));
                return;
            }

            if (string.IsNullOrEmpty(_subtitleViewModel.WorkContext.OpenedCaptionAsset?.Id))
                _browser.Main.ShowMcmDeployAndAssetCreateDialog();
            else
                _browser.Main.ShowMcmDeployDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
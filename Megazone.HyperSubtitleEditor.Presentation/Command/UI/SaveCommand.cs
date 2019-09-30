using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SaveCommand : DependencyObject, ICommand
    {
        private readonly FileManager _fileManager;
        private readonly WorkBarViewModel _workBar;
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitleViewModel;

        public SaveCommand()
        {
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab && _subtitleViewModel.Tabs.Any(tab => tab.CheckDirty());
        }

        public async void Execute(object parameter)
        {
            // 현재 편집 정보가 온라인 정보인지 
            if (_workBar.IsOnlineData)
            {
                // 게시하기.
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
            else
            {
                MessageCenter.Instance.Send(new Subtitle.SaveAsMessage(this));
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

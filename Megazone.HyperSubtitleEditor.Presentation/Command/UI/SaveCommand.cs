using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SaveCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly FileManager _fileManager;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBar;

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
                //// 게시하기.
                //_browser.Main.LoadingManager.Show();
                //var uploadInputPath = await _workBarViewModel.GetUploadInputPathAsync();
                //_browser.Main.LoadingManager.Hide();
                //_workBarViewModel.SetUploadInputPath(uploadInputPath);
                //if (string.IsNullOrEmpty(uploadInputPath))
                //{
                //    // 메시지 처리.
                //    // 게시할수 없음.
                //    _browser.ShowConfirmWindow(
                //        new ConfirmWindowParameter(Resource.CNT_ERROR, Resource.MSG_UPLOAD_FAIL,
                //            MessageBoxButton.OK,
                //            Application.Current.MainWindow));
                //    return;
                //}

                if (string.IsNullOrEmpty(_workBar.CaptionAssetItem?.Id))
                    _browser.Main.ShowMcmDeployAndAssetCreateDialog();
                else
                    _browser.Main.ShowMcmDeployDialog();
            }
            else
            {
                // 로컬 파일로 새로 생성할 경우,
                _browser.Main.ShowMcmDeployAndAssetCreateDialog();

                //MessageCenter.Instance.Send(new SubtitleEditor.SaveAsMessage(this));
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
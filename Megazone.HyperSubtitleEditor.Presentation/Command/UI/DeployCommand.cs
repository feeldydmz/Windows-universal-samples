using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class DeployCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBarViewModel;

        public DeployCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab && _subtitleViewModel.Tabs.Any(tab => tab.CheckDirty());
        }

        public async void Execute(object parameter)
        {
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
            //            MessageBoxButton.OK));
            //    return;
            //}

            if (string.IsNullOrEmpty(_workBarViewModel.CaptionAssetItem?.Id))
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
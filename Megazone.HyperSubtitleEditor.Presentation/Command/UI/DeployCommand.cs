using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class DeployCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitleViewModel;

        public DeployCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab && _subtitleViewModel.Tabs.Any(tab => tab.CheckDirty());
        }

        public async void Execute(object parameter)
        {
            _browser.Main.LoadingManager.Show();
            var uploadInputPath = await _subtitleViewModel.WorkContext.GetUploadInputPathAsync();
            _browser.Main.LoadingManager.Hide();
            _subtitleViewModel.WorkContext.SetUploadInputPath(uploadInputPath);
            if (string.IsNullOrEmpty(uploadInputPath))
            {
                // �޽��� ó��.
                // �Խ��Ҽ� ����.
                // [resource]
                _browser.ShowConfirmWindow(
                    new ConfirmWindowParameter("����", "���ε� ���� ���� ��ȸ�� �����Ͽ����ϴ�.\n�����ڿ��� �����Ͻʽÿ�.",
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
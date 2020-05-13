using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ResetWorkBarCommand : DependencyObject, ICommand
    {
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBarViewModel;
        private readonly IBrowser _browser;

        public ResetWorkBarCommand()
        {
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            if (_workBarViewModel.CaptionAssetItem == null && _workBarViewModel.VideoItem == null)
                return false;

            if (_subtitleViewModel.HasTab)
                return _subtitleViewModel.Tabs?.Any(tab => tab.CheckDirty() || tab.Caption == null) ?? false;

            return false;
        }

        public void Execute(object parameter)
        {
            if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                    Resource.MSG_CAPTION_ASSET_RESET_CONFIRM,
                    MessageBoxButton.OKCancel,
                    Application.Current.MainWindow,
                    TextAlignment.Center)) == MessageBoxResult.Cancel)
            {
                return;
            }

            MessageCenter.Instance.Send(new CloudMedia.CaptionResetMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

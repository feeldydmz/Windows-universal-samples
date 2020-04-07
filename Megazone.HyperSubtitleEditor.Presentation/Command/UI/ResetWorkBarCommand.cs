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
        private readonly IBrowser _browser;

        public ResetWorkBarCommand()
        {
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab && _subtitleViewModel.Tabs.Any(tab => tab.CheckDirty());
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

            MessageCenter.Instance.Send(new CloudMedia.CaptionResetRequestedMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

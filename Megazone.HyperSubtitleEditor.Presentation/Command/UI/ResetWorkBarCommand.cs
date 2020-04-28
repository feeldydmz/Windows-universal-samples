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
            var returnValue = _subtitleViewModel.Tabs?.Any(tab =>
            {
                //로컬 : 로컬 파일 경로를 가지고 있는 것
                //온라인 : CaptionAssetId 를 가지고 있는 것
                //새캡션생성 : 로컬 경로와 CaptionAssetId 가 둘다 없다면 새캡션생성으로 만들어진 것(reset 버튼 비활성화)
                if (!string.IsNullOrEmpty(tab.FilePath) || !string.IsNullOrEmpty(tab.CaptionAssetId))
                {
                    return tab.CheckDirty();
                }
                else
                {
                    return false;
                }
            });

            return returnValue?? false;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class McmDeployCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public McmDeployCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }


        public bool CanExecute(object parameter)
        {
            //return AppContext.Job != null && !string.IsNullOrEmpty(RegionManager.Instance.Current?.API);
            return WorkContext.Video != null;
        }

        public void Execute(object parameter)
        {
            //MessageCenter.Instance.Send(new Subtitle.DeployRequestedMessage(this));

            //TODO: 게시하기 전에 저장이 안되어 있다면, 로컬 저장을 하도록 한다.
            if (WorkContext.IsModified)
            {
                _browser.ShowConfirmWindow(new Infrastructure.View.ConfirmWindowParameter("경고", "변경된 내용을 저장한 후, 게시 하십시오.", MessageBoxButton.OK));
                return;
            }
            _browser.Main.ShowMcmDeployConfirmDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

}

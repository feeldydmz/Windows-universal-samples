using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
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
            return true;
        }

        public void Execute(object parameter)
        {
            //MessageCenter.Instance.Send(new Subtitle.DeployRequestedMessage(this));
            _browser.Main.ShowMcmDeployConfirmDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

}

using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class DeployCommand : DependencyObject, ICommand
    {
        public bool CanExecute(object parameter)
        {
            return AppContext.Job != null && !string.IsNullOrEmpty(RegionManager.Instance.Current?.API);
        }

        public void Execute(object parameter)
        {
            MessageCenter.Instance.Send(new Subtitle.DeployRequestedMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
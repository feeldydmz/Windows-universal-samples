using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CloseTabCommand : DependencyObject, ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is SubtitleTabItemViewModel tab)
                MessageCenter.Instance.Send(new Message.SubtitleEditor.CloseTabMessage(this, tab));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
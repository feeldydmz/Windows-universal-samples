using System;
using System.Windows;
using System.Windows.Input;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ExitApplicationCommand : DependencyObject, ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
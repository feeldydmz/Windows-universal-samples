using System;
using System.Windows;
using System.Windows.Input;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Command
{
    public class CloseWindowCommand : DependencyObject, ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = parameter as Window;
            window?.Close();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
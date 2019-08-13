using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ReplaceTextCommand : DependencyObject, ICommand
    {
        public ReplaceTextCommand()
        {
            ViewModel = Bootstrapper.Container.Resolve<FindAndReplaceViewModel>();
        }

        private FindAndReplaceViewModel ViewModel { get; }

        public bool CanExecute(object parameter)
        {
            return ViewModel.CanReplace();
        }

        public void Execute(object parameter)
        {
            ViewModel.Replace();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
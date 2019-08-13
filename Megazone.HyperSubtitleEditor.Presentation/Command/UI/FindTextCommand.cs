using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class FindTextCommand : DependencyObject, ICommand
    {
        public FindTextCommand()
        {
            ViewModel = Bootstrapper.Container.Resolve<FindAndReplaceViewModel>();
        }

        private FindAndReplaceViewModel ViewModel { get; }

        public bool CanExecute(object parameter)
        {
            return ViewModel.CanFind();
        }

        public void Execute(object parameter)
        {
            ViewModel.Find();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
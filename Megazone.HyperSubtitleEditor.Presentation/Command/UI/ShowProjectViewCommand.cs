using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ShowProjectViewCommand : DependencyObject, ICommand
    {
        private readonly ProjectViewModel _projectViewModel;
        public ShowProjectViewCommand()
        {
            _projectViewModel = Bootstrapper.Container.Resolve<ProjectViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return !_projectViewModel.IsProjectViewVisible;
        }

        public void Execute(object parameter)
        {
            _projectViewModel.Show();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

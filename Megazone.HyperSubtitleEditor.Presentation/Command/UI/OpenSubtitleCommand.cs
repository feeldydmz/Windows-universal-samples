using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class OpenSubtitleCommand : DependencyObject, ICommand
    {
        private readonly WorkBarViewModel _workBarViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        public OpenSubtitleCommand()
        {
            _workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _workBarViewModel.CanImportFile();
        }

        public void Execute(object parameter)
        {
            _subtitleViewModel.OnImportSubtitleFile();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
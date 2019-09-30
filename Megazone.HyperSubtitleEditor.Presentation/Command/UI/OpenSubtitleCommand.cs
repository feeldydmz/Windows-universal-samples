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
        private readonly IBrowser _browser;
        private readonly WorkBarViewModel _workBarViewModel;

        public OpenSubtitleCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _workBarViewModel.CanImportFile();
        }

        public void Execute(object parameter)
        {
            _browser.Main.ShowOpenSubtitleDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
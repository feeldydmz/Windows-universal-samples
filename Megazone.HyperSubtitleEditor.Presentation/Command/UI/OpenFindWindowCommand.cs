using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class OpenFindWindowCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public OpenFindWindowCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        private SubtitleViewModel SubtitleViewModel { get; } = Bootstrapper.Container.Resolve<SubtitleViewModel>();

        public bool CanExecute(object parameter)
        {
            return SubtitleViewModel?.SelectedTab?.Rows?.Any() ?? false;
        }

        public void Execute(object parameter)
        {
            _browser.Main.ShowFindDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
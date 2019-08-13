using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ShowSettingsViewCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public ShowSettingsViewCommand()
        {
            if (this.IsInDesignMode())
                return;
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _browser.Main.ShowSettingsDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
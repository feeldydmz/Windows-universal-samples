using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ShowOpenVideoAddressWindowCommand : DependencyObject, ICommand
    {
        private IBrowser Browser { get; } = Bootstrapper.Container.Resolve<IBrowser>();

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var openTypeString = "";
            if (parameter is string paramString)
            {
                openTypeString = paramString;
            }

            Browser.Main.ShowOpenVideoAddressWindow(openTypeString);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
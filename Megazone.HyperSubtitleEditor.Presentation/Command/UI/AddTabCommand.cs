using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class AddTabCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public AddTabCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _browser.Main.ShowAddAndEditSubtitleDialog(SubtitleDialogViewMode.Add, null, Resource.CNT_ADD_NEW_CAPTION);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
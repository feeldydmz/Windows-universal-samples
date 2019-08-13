using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class EditTabCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public EditTabCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!(parameter is ISubtitleTabItemViewModel))
                return;

            _browser.Main.ShowAddAndEditSubtitleDialog(SubtitleDialogViewMode.Edit,
                (ISubtitleTabItemViewModel) parameter,
                Resource.CNT_EDIT_CAPTION_INFO);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
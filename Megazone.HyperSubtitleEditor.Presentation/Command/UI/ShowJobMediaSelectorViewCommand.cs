using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Unity;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ShowJobMediaSelectorViewCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public ShowJobMediaSelectorViewCommand()
        {
            if (this.IsInDesignMode())
                return;
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return AppContext.Job != null;
        }

        public void Execute(object parameter)
        {
            _browser.Main.JobMediaItemSelector.Show();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
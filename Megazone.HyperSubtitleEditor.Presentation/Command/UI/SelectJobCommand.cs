using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Unity;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SelectJobCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public SelectJobCommand()
        {
            if (this.IsInDesignMode())
                return;
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(AppContext.Config.PipelineId) &&
                   !string.IsNullOrEmpty(AppContext.Config.ProfileId);
        }

        public void Execute(object parameter)
        {
            _browser.Main.JobSelector.Show();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
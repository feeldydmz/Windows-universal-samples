using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CopyTabCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitleViewModel;

        public CopyTabCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel?.Tabs?.Any() ?? false;
        }

        public void Execute(object parameter)
        {
            _browser.Main.ShowCopySubtitleDialog(_subtitleViewModel.Tabs);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
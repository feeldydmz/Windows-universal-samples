using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class GoToSubtitleNumberCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;

        public GoToSubtitleNumberCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        private SubtitleViewModel SubtitleViewModel { get; } = Bootstrapper.Container.Resolve<SubtitleViewModel>();

        public bool CanExecute(object parameter)
        {
            return SubtitleViewModel.SelectedTabRowsCount > 0;
        }

        public void Execute(object parameter)
        {
            _browser.Main.SubtitleView.ShowGoToLineDialog(SubtitleViewModel.SelectedTabRowsCount);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SelectAllRowsCommand : DependencyObject, ICommand
    {
        private SubtitleViewModel SubtitleViewModel { get; } = Bootstrapper.Container.Resolve<SubtitleViewModel>();

        public bool CanExecute(object parameter)
        {
            return SubtitleViewModel.SelectedTabRowsCount > 0;
        }

        public void Execute(object parameter)
        {
            MessageCenter.Instance.Send(new SubtitleView.SelectAllRowsMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
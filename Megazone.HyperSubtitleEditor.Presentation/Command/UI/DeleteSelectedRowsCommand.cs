using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class DeleteSelectedRowsCommand : DependencyObject, ICommand
    {
        private SubtitleViewModel SubtitleViewModel { get; } = Bootstrapper.Container.Resolve<SubtitleViewModel>();

        public bool CanExecute(object parameter)
        {
            return SubtitleViewModel.HasSelectedRows;
        }

        public void Execute(object parameter)
        {
            MessageCenter.Instance.Send(new SubtitleView.DeleteSelectRowsMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
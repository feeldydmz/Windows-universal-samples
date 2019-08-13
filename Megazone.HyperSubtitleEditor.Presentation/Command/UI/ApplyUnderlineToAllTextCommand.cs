using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ApplyUnderlineToAllTextCommand : DependencyObject, ICommand
    {
        private readonly SubtitleViewModel _subtitleViewModel;

        public ApplyUnderlineToAllTextCommand()
        {
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            var model = _subtitleViewModel.SelectedItem;
            return model?.Texts?.Any() ?? false;
        }

        public void Execute(object parameter)
        {
            MessageCenter.Instance.Send(new SubtitleView.ApplyUnderlineToAllTextMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
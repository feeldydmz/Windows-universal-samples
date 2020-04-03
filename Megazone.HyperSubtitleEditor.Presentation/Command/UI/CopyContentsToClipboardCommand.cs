using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CopyContentsToClipboardCommand : DependencyObject, ICommand
    {
        private readonly SubtitleViewModel _subtitleViewModel;

        public CopyContentsToClipboardCommand()
        {
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel?.SelectedTab?.SelectedRow != null;
        }

        public void Execute(object parameter)
        {
            MessageCenter.Instance.Send(new Message.SubtitleEditor.CopyContentsToClipboardMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SaveAsCommand : DependencyObject, ICommand
    {
        private readonly FileManager _fileManager;

        public SaveAsCommand()
        {
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MessageCenter.Instance.Send(new Message.SubtitleEditor.SaveAsMessage(this));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
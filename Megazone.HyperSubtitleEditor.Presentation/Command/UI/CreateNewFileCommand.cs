using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CreateNewFileCommand : DependencyObject, ICommand
    {
        private readonly FileManager _fileManager;

        public CreateNewFileCommand()
        {
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _fileManager.CreateNewFile(".vtt", "WebVtt files (.vtt)|*.vtt");
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
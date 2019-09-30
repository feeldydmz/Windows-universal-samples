using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Extension;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CreateNewCommand : DependencyObject, ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var fileFullPath = $"{AppDomain.CurrentDomain.BaseDirectory}{this.GetApplicationName()}";
            if (File.Exists(fileFullPath))
                Process.Start(fileFullPath);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
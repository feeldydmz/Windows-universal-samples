using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class OpenLogFolderCommand : DependencyObject, ICommand
    {
        private readonly ILogger _logger;

        public OpenLogFolderCommand()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                Process.Start(Directory.Exists(this.LogDirPath()) ? this.LogDirPath() : this.AppDataPath());
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
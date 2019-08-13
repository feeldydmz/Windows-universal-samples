using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Log;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension;
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
                if (Directory.Exists(this.HyperSubtitleEditorAppDataPath() + "/logs"))
                    Process.Start(this.HyperSubtitleEditorAppDataPath() + "/logs");
                else
                    Process.Start(this.HyperSubtitleEditorAppDataPath());
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
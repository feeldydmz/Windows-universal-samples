using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CreateNewWindowCommand : DependencyObject, ICommand
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

        public event EventHandler CanExecuteChanged;
    }
}

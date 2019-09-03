using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class DownloadExcelTemplet : DependencyObject, ICommand
    {
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;

        public DownloadExcelTemplet()
        {
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
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
                var savePath =
                    _fileManager.OpenSaveFileDialog($@"{this.MyDocuments()}\HyperSubtitleEditor_Caption_Format.xlsx",
                        "Excel files (*.xlsx)|*.xlsx");

                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName =
                    "Megazone.HyperSubtitleEditor.Presentation.Excel.HyperSubtitleEditor_Caption_Format.xlsx";

                var fileStream = File.Create(savePath);

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new FileNotFoundException();

                    stream.CopyTo(fileStream);
                    fileStream.Close();
                }

                Process.Start("explorer.exe", Path.GetDirectoryName(savePath));
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
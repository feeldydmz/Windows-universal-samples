using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Microsoft.Win32;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class FileManager
    {
        private readonly ExcelService _excelService;
        private readonly ILogger _logger;

        public FileManager(ILogger logger, ExcelService excelService)
        {
            _logger = logger;
            _excelService = excelService;
        }

        public bool CreateNewFile(string defaultExtension, string filter)
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = defaultExtension,
                Filter = filter
            };
            if (dlg.ShowDialog() != true) return false;
            var fileName = dlg.FileName;
            using (File.Create(fileName))
            {
                MessageCenter.Instance.Send(
                    new Message.SubtitleEditor.FileOpenedMessage(this, new FileOpenedMessageParameter
                    {
                        FilePath = fileName
                    }));
            }

            return true;
        }

        public string OpenFile(string filter, string initialDirectory = null)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = string.IsNullOrEmpty(initialDirectory) ?
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : initialDirectory,
                Filter = filter
            };

            return  openFileDialog.ShowDialog() != true ? string.Empty : openFileDialog.FileName;
        }

        public string OpenSaveFileDialog(string filePath, string filter, string defaultFileName = null)
        {
            var dlg = new SaveFileDialog();
            if (!string.IsNullOrEmpty(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                dlg.FileName = fileInfo.Name;
                dlg.DefaultExt = fileInfo.Extension;
            }

            if (!string.IsNullOrEmpty(defaultFileName))
                dlg.FileName = defaultFileName;
            if (!string.IsNullOrEmpty(filter))
                dlg.Filter = filter;
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public bool Save(string filePath, string text, Encoding encoding = null)
        {
            try
            {
                encoding = encoding ?? Encoding.Default;
                File.WriteAllText(filePath, text, encoding);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }

            return false;
        }

        public bool ExportExcel(IList<Subtitle> subtitles, string saveFilePath)
        {
            if (!subtitles.Any() && string.IsNullOrEmpty(saveFilePath))
                return false;

            return _excelService.CreateFile(subtitles, saveFilePath);
        }
    }
}
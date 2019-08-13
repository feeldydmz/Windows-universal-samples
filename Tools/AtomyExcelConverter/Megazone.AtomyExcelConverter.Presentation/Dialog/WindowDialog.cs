using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Megazone.Core.IoC;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Megazone.AtomyExcelConverter.Presentation.Dialog
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class WindowDialog
    {
        public IEnumerable<string> OpenFileDialog(string title, string initialDirectory, string filter, bool isMultiselect)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = initialDirectory,
                Filter = filter,
                Multiselect = isMultiselect
            };
            return openFileDialog.ShowDialog() != true ? null : openFileDialog.FileNames;
        }

        public string OpenFolderDialog(string description, Environment.SpecialFolder initialDirectory = Environment.SpecialFolder.Desktop)
        {

            var folderBrowserDialog = new FolderBrowserDialog
            {
                RootFolder = initialDirectory,
                Description = description
            };

            return folderBrowserDialog.ShowDialog() == DialogResult.OK ? folderBrowserDialog.SelectedPath : string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Megazone.AtomyExcelConverter.Presentation.Dialog;
using Megazone.AtomyExcelConverter.Presentation.Excel;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.Browser;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.View;
using Megazone.AtomyExcelConverter.Presentation.ItemViewModel;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Windows.Mvvm;

namespace Megazone.AtomyExcelConverter.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class MainViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly ExcelService _excelService;
        private readonly IBrowser _browser;
        private readonly WindowDialog _windowDialog;

        private ObservableCollection<ExcelFileItemViewModel> _excelFileItems;
        private ICommand _addExcelFilesCommand;
        private ICommand _deleteExcelCommand;
        private string _saveFolderPath;
        private ICommand _changeSaveFolderPathCommand;
        private ICommand _convertExcelFilesCommand;
        private ICommand _deleteAllExcelFilesCommand;
        private string _saveFileName;
        private ICommand _openExcelFilePath;
        private ICommand _openSaveDirctoryCommand;

        public MainViewModel(ILogger logger, ExcelService excelService, IBrowser browser, WindowDialog windowDialog)
        {
            _logger = logger;
            _excelService = excelService;
            _browser = browser;
            _windowDialog = windowDialog;
            _excelFileItems = new ObservableCollection<ExcelFileItemViewModel>();
            SaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AtomyConvertExcelFiles";
        }

        public IList<ExcelFileItemViewModel> ExcelFileItems
        {
            get { return _excelFileItems; }
            private set { Set(ref _excelFileItems, value as ObservableCollection<ExcelFileItemViewModel>); }
        }

        public string SaveFolderPath
        {
            get { return _saveFolderPath; }
            set { Set(ref _saveFolderPath, value); }
        }

        public string SaveFileName
        {
            get { return _saveFileName; }
            set { Set(ref _saveFileName, value); }
        }

        public ICommand AddExcelFilesCommand
        {
            get { return _addExcelFilesCommand = _addExcelFilesCommand ?? new RelayCommand(AddExcelFiles); }
        }

        public ICommand DeleteExcelFileCommand
        {
            get { return _deleteExcelCommand = _deleteExcelCommand ?? new RelayCommand<ExcelFileItemViewModel>(DeleteExcelFile); }
        }

        public ICommand OpenExcelFilePathCommand
        {
            get { return _openExcelFilePath = _openExcelFilePath ?? new RelayCommand<ExcelFileItemViewModel>(
                                                  excelFile =>
                                                  {
                                                      OpenDirectory(Path.GetDirectoryName(excelFile.ExcelInfo.FileFullPath));
                                                  }); }
        }

        public ICommand DeleteAllExcelFilesCommand
        {
            get { return _deleteAllExcelFilesCommand = _deleteAllExcelFilesCommand ?? new RelayCommand(DeleteAllExcelFiles); }
        }

        public ICommand ChangeSaveFolderPathCommand
        {
            get { return _changeSaveFolderPathCommand = _changeSaveFolderPathCommand ?? new RelayCommand(ChangeSaveFolderPath); }
        }
        public ICommand ConvertExcelFilesCommand
        {
            get { return _convertExcelFilesCommand = _convertExcelFilesCommand ?? new RelayCommand(ConvertExcelFiles, CanConvertExcelFiles); }
        }

        public ICommand OpenSaveDirctoryCommand
        {
            get { return _openSaveDirctoryCommand = _openSaveDirctoryCommand ?? new RelayCommand(() => { OpenDirectory(SaveFolderPath); }); }
        }

        private void OpenDirectory(string directoryPath)
        {
            try
            {
                Process.Start("explorer.exe", directoryPath);
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                _browser.ShowConfirmWindow(new ConfirmWindowParameter("알림", directoryPath + "경로를 열 수 없습니다.", MessageBoxButton.OK));
            }
        }

        private void AddExcelFiles()
        {
            var selectFolder = _windowDialog.OpenFolderDialog("추가 할 엑셀 파일이 있는 폴더를 선택해주세요.");

            if (string.IsNullOrEmpty(selectFolder))
                return;

            var currentDirectoryExcelFiles = Directory.GetFiles(selectFolder, "*.xlsx", SearchOption.TopDirectoryOnly);

            SaveFileName = Path.GetFileName(selectFolder) + ".xlsx";

            if (!currentDirectoryExcelFiles.IsNotNullOrAny())
            {
                _browser.ShowConfirmWindow(new ConfirmWindowParameter("알림", "추가 할 Excel(*.xlsx) 파일이 없습니다.", MessageBoxButton.OK));
                return;
            }
            
            var notExistPaths = currentDirectoryExcelFiles.Where(i => !ExcelFileItems.Any(j => j.ExcelInfo.FileName.Equals(Path.GetFileName(i)))).ToList();

            var existNames = currentDirectoryExcelFiles.Except(notExistPaths).ToList();

            if (existNames.IsNotNullOrAny())
                _browser.ShowConfirmWindow(new ConfirmWindowParameter("알림", CreateMessageString(existNames) + "이미 추가되어 있는 파일입니다.",
                    MessageBoxButton.OK));

            AddExcelFileItemViewModel(notExistPaths);
        }

        private async void AddExcelFileItemViewModel(IEnumerable<string> fileFullPaths)
        {
            _browser.Main.LoadingManager.Show();

            var result = await this.CreateTask(() => _excelService.LoadExcelInfos(fileFullPaths).ToList());

            if (result.IsNotNullOrAny())
            {
                foreach (var item in result)
                {
                    ExcelFileItems.Add(new ExcelFileItemViewModel(item));
                }
            }

            CommandManager.InvalidateRequerySuggested();
            _browser.Main.LoadingManager.Hide();
        }

        private void DeleteExcelFile(ExcelFileItemViewModel excelFile)
        {
            ExcelFileItems.Remove(excelFile);
        }

        private void DeleteAllExcelFiles()
        {
            ExcelFileItems.Clear();
        }

        private bool CanConvertExcelFiles()
        {
            return ExcelFileItems.IsNotNullOrAny();
        }

        private async void ConvertExcelFiles()
        {
            _browser.Main.LoadingManager.Show();
            var errorMessages = await this.CreateTask(() =>
                       {
                           IList<ExcelInfo> excelInfos = ExcelFileItems.Select(item => item.ExcelInfo).ToList();
                           return _excelService.CreateExcelFile(excelInfos, SaveFolderPath, SaveFileName).ToList();
                       });

            if (errorMessages.IsNotNullOrAny())
                _browser.ShowConfirmWindow(new ConfirmWindowParameter("오류", CreateMessageString(errorMessages), MessageBoxButton.OK));

            Process.Start("explorer.exe", SaveFolderPath);
            _browser.Main.LoadingManager.Hide();
        }

        private void ChangeSaveFolderPath()
        {
            var saveFolderPath = _windowDialog.OpenFolderDialog("저장할 폴더를 선택해주세요.");
            if (string.IsNullOrEmpty(saveFolderPath))
                return;
            SaveFolderPath = saveFolderPath;
        }

        private static string CreateMessageString(IEnumerable<string> messages)
        {
            var message = new StringBuilder();

            foreach (var item in messages.ToList())
            {
                message.Append(item + Environment.NewLine);
            }

            return message.ToString();
        }
    }
}

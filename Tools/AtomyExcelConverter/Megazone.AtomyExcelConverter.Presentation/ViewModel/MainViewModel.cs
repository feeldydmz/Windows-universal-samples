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
using Microsoft.Win32;

namespace Megazone.AtomyExcelConverter.Presentation.ViewModel
{
	[Inject(Scope = LifetimeScope.Transient)]
	internal class MainViewModel : ViewModelBase
	{
		private const double DEFAULT_FRAME_PER_SECOND = 29.97d;
		private readonly IBrowser _browser;
		private readonly ExcelService _excelService;
		private readonly ILogger _logger;
		private readonly WindowDialog _windowDialog;
		private ICommand _changeSaveFolderPathCommand;
		private ICommand _convertExcelFilesCommand;
		private ICommand _deleteAllExcelFilesCommand;
		private ICommand _deleteExcelCommand;

		private ObservableCollection<ExcelFileItemViewModel> _excelFileItems;

		private string _framePerSecond = DEFAULT_FRAME_PER_SECOND.ToString();
		private ICommand _importExcelFilesCommand;

		private ICommand _importExcelFilesFromFolderCommand;

		private bool _isConverterV1;
		private bool _isConverterV2 = true;
		private ICommand _openExcelFilePath;
		private ICommand _openSaveDirctoryCommand;
		private string _saveFileName;
		private string _saveFolderPath;

		public MainViewModel(ILogger logger, ExcelService excelService, IBrowser browser, WindowDialog windowDialog)
		{
			_logger = logger;
			_excelService = excelService;
			_browser = browser;
			_windowDialog = windowDialog;
			_excelFileItems = new ObservableCollection<ExcelFileItemViewModel>();
			SaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
			                 "\\AtomyConvertExcelFiles";
		}

		public bool IsConverterV1
		{
			get => _isConverterV1;
			set => Set(ref _isConverterV1, value);
		}

		public bool IsConverterV2
		{
			get => _isConverterV2;
			set => Set(ref _isConverterV2, value);
		}

		public IList<ExcelFileItemViewModel> ExcelFileItems
		{
			get => _excelFileItems;
			private set => Set(ref _excelFileItems, value as ObservableCollection<ExcelFileItemViewModel>);
		}

		public string SaveFolderPath
		{
			get => _saveFolderPath;
			set => Set(ref _saveFolderPath, value);
		}

		public string SaveFileName
		{
			get => _saveFileName;
			set => Set(ref _saveFileName, value);
		}

		public ICommand ImportExcelFilesFromFolderCommand
		{
			get
			{
				return _importExcelFilesFromFolderCommand =
					_importExcelFilesFromFolderCommand ?? new RelayCommand(ImportExcelFilesFromFolder);
			}
		}

		public ICommand ImportExcelFilesCommand
		{
			get { return _importExcelFilesCommand = _importExcelFilesCommand ?? new RelayCommand(ImportExcelFiles); }
		}

		public ICommand DeleteExcelFileCommand
		{
			get
			{
				return _deleteExcelCommand =
					_deleteExcelCommand ?? new RelayCommand<ExcelFileItemViewModel>(DeleteExcelFile);
			}
		}

		public ICommand OpenExcelFilePathCommand
		{
			get
			{
				return _openExcelFilePath = _openExcelFilePath ?? new RelayCommand<ExcelFileItemViewModel>(
					                            excelFile =>
					                            {
						                            OpenDirectory(
							                            Path.GetDirectoryName(excelFile.DisplayFileFullPath));
					                            });
			}
		}

		public ICommand DeleteAllExcelFilesCommand
		{
			get
			{
				return _deleteAllExcelFilesCommand =
					_deleteAllExcelFilesCommand ?? new RelayCommand(DeleteAllExcelFiles);
			}
		}

		public ICommand ChangeSaveFolderPathCommand
		{
			get
			{
				return _changeSaveFolderPathCommand =
					_changeSaveFolderPathCommand ?? new RelayCommand(ChangeSaveFolderPath);
			}
		}

		public ICommand ConvertExcelFilesCommand
		{
			get
			{
				return _convertExcelFilesCommand =
					_convertExcelFilesCommand ?? new RelayCommand(ConvertExcelFiles, CanConvertExcelFiles);
			}
		}

		public ICommand OpenSaveDirctoryCommand
		{
			get
			{
				return _openSaveDirctoryCommand =
					_openSaveDirctoryCommand ?? new RelayCommand(() => { OpenDirectory(SaveFolderPath); });
			}
		}

		public string FramePerSecond
		{
			get => _framePerSecond;
			set => Set(ref _framePerSecond, value);
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
				_browser.ShowConfirmWindow(new ConfirmWindowParameter("알림", directoryPath + "경로를 열 수 없습니다.",
					MessageBoxButton.OK));
			}
		}


		private void ImportExcelFiles()
		{
			var dialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "Excel files (*.xlsx)|*.xlsx",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (dialog.ShowDialog() != true)
				return;

			var notExistPaths = dialog.FileNames
				.Where(i => !ExcelFileItems.Any(j => j.DisplayFileFullPath.Equals(i))).ToList();

			var existNames = dialog.FileNames.Except(notExistPaths).ToList();

			if (existNames.IsNotNullOrAny())
				_browser.ShowConfirmWindow(new ConfirmWindowParameter("알림",
					CreateMessageString(existNames) + "이미 추가되어 있는 파일입니다.",
					MessageBoxButton.OK));

			AddExcelFileItemViewModel(notExistPaths);
		}

		private void ImportExcelFilesFromFolder()
		{
			var selectFolder = _windowDialog.OpenFolderDialog("추가 할 엑셀 파일이 있는 폴더를 선택해주세요.");

			if (string.IsNullOrEmpty(selectFolder))
				return;

			var currentDirectoryExcelFiles = Directory.GetFiles(selectFolder, "*.xlsx", SearchOption.TopDirectoryOnly);

			SaveFileName = Path.GetFileName(selectFolder) + ".xlsx";

			if (!currentDirectoryExcelFiles.IsNotNullOrAny())
			{
				_browser.ShowConfirmWindow(new ConfirmWindowParameter("알림", "추가 할 Excel(*.xlsx) 파일이 없습니다.",
					MessageBoxButton.OK));
				return;
			}

			var notExistPaths = currentDirectoryExcelFiles
				.Where(i => !ExcelFileItems.Any(j => j.DisplayFileFullPath.Equals(i))).ToList();

			var existNames = currentDirectoryExcelFiles.Except(notExistPaths).ToList();

			if (existNames.IsNotNullOrAny())
				_browser.ShowConfirmWindow(new ConfirmWindowParameter("알림",
					CreateMessageString(existNames) + "이미 추가되어 있는 파일입니다.",
					MessageBoxButton.OK));

			AddExcelFileItemViewModel(notExistPaths);
		}

		private double GetFramePerSecond()
		{
			return double.TryParse(_framePerSecond, out var result) ? result : DEFAULT_FRAME_PER_SECOND;
		}

		private void AddExcelFileItemViewModel(IEnumerable<string> fileFullPaths)
		{
			foreach (var path in fileFullPaths)
				ExcelFileItems.Add(new ExcelFileItemViewModel(new FileInfo(path).Name, path));


			if(string.IsNullOrEmpty(_saveFileName) && ExcelFileItems.Any())
				SaveFileName = ExcelFileItems.First().DisplayName;

			CommandManager.InvalidateRequerySuggested();
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

		private string GetConverterVersion()
		{
			if (IsConverterV1)
				return "V1";
			if (IsConverterV2)
				return "V2";
			throw new ArgumentException("Converter engine version must be selected");
		}

		private async void ConvertExcelFiles()
		{
			_browser.Main.LoadingManager.Show();

			try
			{
				var converterVersion = GetConverterVersion();
				if (string.IsNullOrEmpty(converterVersion))
				{
					_browser.ShowConfirmWindow(new ConfirmWindowParameter("오류",
						"Converter engine version must be selected.",
						MessageBoxButton.OK));
					return;
				}

				var errorMessages = await this.CreateTask(() =>
				{
					var excelInfos = ExcelFileItems.Select(item =>
							_excelService.LoadExcelInfo(item.DisplayName, item.DisplayFileFullPath, GetFramePerSecond(),
								converterVersion))
						.ToList();
					return _excelService.CreateExcelFile(excelInfos, SaveFolderPath, SaveFileName).ToList();
				});

				if (errorMessages.IsNotNullOrAny())
					_browser.ShowConfirmWindow(new ConfirmWindowParameter("오류", CreateMessageString(errorMessages),
						MessageBoxButton.OK));

				Process.Start("explorer.exe", SaveFolderPath);
			}
			catch (Exception ex)
			{
				_logger.Error.Write(ex.Message);
			}
			finally
			{
				_browser.Main.LoadingManager.Hide();
			}
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

			foreach (var item in messages.ToList()) message.Append(item + Environment.NewLine);

			return message.ToString();
		}
	}
}
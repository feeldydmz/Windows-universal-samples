using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class ImportExcelViewModel : ViewModelBase
    {
        private readonly ExcelService _excelService;
        private readonly FileManager _fileManager;
        private readonly LanguageLoader _languageLoader;
        private string _excelFilePath;
        private ICommand _importChooseExcelSheetCommand;
        private ICommand _importExcelFileCommand;
        private ICommand _loadedCommand;
        private IList<ImportExcelItemViewModel> _sheets;

        public ImportExcelViewModel(FileManager fileManager, ExcelService excelService, LanguageLoader languageLoader)
        {
            _fileManager = fileManager;
            _excelService = excelService;
            _languageLoader = languageLoader;
            _sheets = new ObservableCollection<ImportExcelItemViewModel>();
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public ICommand ImportExcelFileCommand
        {
            get { return _importExcelFileCommand = _importExcelFileCommand ?? new RelayCommand(ImportExcelFile); }
        }

        public ICommand ImportChooseExcelSheetCommand
        {
            get
            {
                return
                    _importChooseExcelSheetCommand =
                        _importChooseExcelSheetCommand ?? new RelayCommand(ImportChooseExcelSheet);
            }
        }

        public string ExcelFilePath
        {
            get => _excelFilePath;
            set => Set(ref _excelFilePath, value);
        }

        public IList<ImportExcelItemViewModel> Sheets
        {
            get => _sheets;
            set => Set(ref _sheets, value);
        }

        public Action CloseAction { get; set; }
        public string InitialFilePath { get; internal set; }

        private void OnLoaded()
        {
            ExcelFilePath = InitialFilePath;
            ImportExcelFromFilePath(ExcelFilePath);
        }

        private void ImportChooseExcelSheet()
        {
            IList<ExcelSheetInfo> sheetInfos = new List<ExcelSheetInfo>();

            foreach (var item in Sheets.ToList())
            {
                if (!item.IsChecked)
                    continue;

                if (string.IsNullOrEmpty(item.Label) ||
                    string.IsNullOrEmpty(item.SelectedLanguage?.LanguageCode) ||
                    string.IsNullOrEmpty(item.SelectedLanguage?.CountryCode))
                    return;


                sheetInfos.Add(new ExcelSheetInfo
                {
                    SheetName = item.SheetName,
                    Label = item.Label,
                    LanguageCode = item.SelectedLanguage.LanguageCode,
                    CountryCode = item.SelectedLanguage.CountryCode,
                    CaptionKind = item.SelectedSubtitleKind,
                    TrackFormat = TrackFormat.WebVtt
                });
            }

            CloseAction?.Invoke();

            MessageCenter.Instance.Send(new Message.Excel.FileImportMessage(this, ExcelFilePath, sheetInfos));
        }

        private void ImportExcelFile()
        {
            ExcelFilePath = _fileManager.OpenFile("Excel files (*.xlsx)|*.xlsx");

            ImportExcelFromFilePath(ExcelFilePath);
        }

        private async void ImportExcelFromFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var worksheetInfos = await this.CreateTask(() => _excelService.GetSheetInfo(filePath)
                .ToList());

            if (worksheetInfos.Any())
                AddToExcelSheetListItems(worksheetInfos);
        }

        private void AddToExcelSheetListItems(IList<ExcelSheetInfo> infos)
        {
            Sheets.Clear();

            try
            {
                foreach (var sheetInfo in infos)
                {
                    var excelItemViewModel = new ImportExcelItemViewModel(_languageLoader)
                    {
                        IsChecked = true, // 기본값이 체크되어 있는 형태.
                        SheetName = sheetInfo.SheetName,
                        Label = sheetInfo.Label,
                        SelectedSubtitleKind = sheetInfo.CaptionKind
                    };

                    excelItemViewModel.SelectedLanguage = excelItemViewModel.Languages.SingleOrDefault(language =>
                        language.LanguageCode.Equals(sheetInfo.LanguageCode) &&
                        language.CountryCode.Equals(sheetInfo.CountryCode));

                    Sheets.Add(excelItemViewModel);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}
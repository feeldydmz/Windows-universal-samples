using Megazone.AtomyExcelConverter.Presentation.Excel;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure;

namespace Megazone.AtomyExcelConverter.Presentation.ItemViewModel
{
    internal class ExcelFileItemViewModel : ViewModelBase
    {
        public ExcelFileItemViewModel(ExcelInfo excelInfo)
        {
            DisplayName = excelInfo.FileName;
            DisplayFileFullPath = excelInfo.FileFullPath;
            ExcelInfo = excelInfo;
        }

        public string DisplayFileFullPath { get; }

        public string DisplayName { get; }

        public ExcelInfo ExcelInfo { get; }
    }
}

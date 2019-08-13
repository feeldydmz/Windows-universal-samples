using System.Collections.Generic;

namespace Megazone.AtomyExcelConverter.Presentation.Excel
{
    internal class ExcelSheetInfo
    {
        public ExcelSheetInfo(string name, IEnumerable<SubtitleItem> datasets)
        {
            SheetName = name;
            Datasets = datasets;
        }

        public string SheetName { get; }

        public IEnumerable<SubtitleItem> Datasets { get; }
    }
}

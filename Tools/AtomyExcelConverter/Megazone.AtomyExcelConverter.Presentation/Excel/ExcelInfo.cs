using System.Collections.Generic;

namespace Megazone.AtomyExcelConverter.Presentation.Excel
{
    internal class ExcelInfo
    {
        public string FileName { get; set; }

        public string FileFullPath { get; set; }

        public IList<ExcelSheetInfo> SheetInfos { get; } = new List<ExcelSheetInfo>();
    }
}

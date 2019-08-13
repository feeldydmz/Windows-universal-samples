using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class Excel
    {
        internal class FileImportMessage : MessageBase
        {
            public FileImportMessage(object sender, string filePath, IList<ExcelSheetInfo> sheetInfos) : base(sender)
            {
                FilePath = filePath;
                SheetInfos = sheetInfos;
            }

            public string FilePath { get; }

            public IList<ExcelSheetInfo> SheetInfos { get; }
        }
    }
}
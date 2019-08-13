using System.Collections.Generic;
using SubtitleModel = Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    public interface IExcelService
    {
        IEnumerable<SubtitleModel.ExcelSheetInfo> GetSheetInfo(string excelPath);

        IEnumerable<SubtitleModel.Subtitle> GetSheetContents(string excelPath,
            IList<SubtitleModel.ExcelSheetInfo> sheetInfos);

        bool CreateFile(IEnumerable<SubtitleModel.Subtitle> subtitles, string saveFolderPath);
    }
}
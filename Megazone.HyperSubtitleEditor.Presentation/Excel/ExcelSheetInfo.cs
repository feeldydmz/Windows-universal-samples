using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Excel
{
    public class ExcelSheetInfo
    {
        public string SheetName { get; set; }
        public string Label { get; set; }
        public string LanguageCode { get; set; }
        public TrackFormat TrackFormat { get; set; }
        public TrackKind TrackKind { get; set; }
    }
}
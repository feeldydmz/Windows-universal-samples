using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle.Model
{
    public class ExcelSheetInfo
    {
        public string SheetName { get; set; }
        public string LanguageCode { get; set; }
        public TrackFormat TrackFormat { get; set; }
        public TrackKind TrackKind { get; set; }
    }
}
using Megazone.Core.VideoTrack;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    internal class FileOpenedMessageParameter
    {
        public string FilePath { get; set; }
        public string Label { get; set; }
        public string LanguageCode { get; set; }
        public string CountryCode { get; set; }
        public CaptionKind Kind { get; set; }
        public SubtitleFormatKind SubtitleFormat { get; set; }
    }
}
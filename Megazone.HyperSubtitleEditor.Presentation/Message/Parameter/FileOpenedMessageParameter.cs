using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    internal class FileOpenedMessageParameter
    {
        public string FilePath { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public string LanguageCode { get; set; }
        public TrackKind Kind { get; set; }
        public string CaptionId { get; set; }
    }
}
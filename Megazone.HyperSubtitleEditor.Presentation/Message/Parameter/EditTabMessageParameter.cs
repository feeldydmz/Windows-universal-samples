using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    internal class EditTabMessageParameter
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string LanguageCode { get; set; }
        public TrackKind Kind { get; set; }
    }
}
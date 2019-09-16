using Megazone.Cloud.Media.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    internal class EditTabMessageParameter
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string LanguageCode { get; set; }
        public CaptionKind Kind { get; set; }
    }
}
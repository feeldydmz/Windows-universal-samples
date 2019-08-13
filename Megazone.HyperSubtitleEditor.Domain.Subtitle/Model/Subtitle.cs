using System.Collections.Generic;
using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle.Model
{
    public class Subtitle
    {
        public string Label { get; set; }
        public string LanguageCode { get; set; }
        public TrackFormat Format { get; set; }
        public TrackKind Kind { get; set; }
        public IList<ISubtitleItem> Datasets { get; set; } = new List<ISubtitleItem>();
    }
}
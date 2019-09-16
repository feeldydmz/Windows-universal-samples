using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.VideoTrack.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.Excel
{
    public class Subtitle
    {
        public string Label { get; set; }
        public string LanguageCode { get; set; }
        public TrackFormat Format { get; set; }
        public CaptionKind Kind { get; set; }
        public IList<SubtitleItem> Datasets { get; set; } = new List<SubtitleItem>();
    }
}
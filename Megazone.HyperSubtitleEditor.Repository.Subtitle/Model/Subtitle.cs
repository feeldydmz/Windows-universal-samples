using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;
using Megazone.HyperSubtitleEditor.Domain.Subtitle.Enum;

namespace Megazone.HyperSubtitleEditor.Repository.Subtitle.Model
{
    internal class Subtitle
    {
        public string CountryCode { get; set; }

        public SubtitleFormat Format { get; set; }

        public IList<ISubtitleItem> Datasets { get; } = new List<ISubtitleItem>();
    }
}

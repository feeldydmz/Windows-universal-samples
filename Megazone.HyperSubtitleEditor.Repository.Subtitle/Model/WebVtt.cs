using System;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;

namespace Megazone.HyperSubtitleEditor.Repository.Subtitle.Model
{
    internal class WebVtt : ISubtitleItem
    {
        public string Number { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Text { get; set; }
    }
}

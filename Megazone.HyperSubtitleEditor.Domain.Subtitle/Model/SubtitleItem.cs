using System;
using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle.Model
{
    public class SubtitleItem : ISubtitleItem
    {
        public SubtitleItem()
        {
            StartTime = TimeSpan.Zero;
            EndTime = TimeSpan.Zero;
        }

        public SubtitleItem(TimeSpan startTime, TimeSpan endTime, IList<IText> texts, string cueSettingData)
        {
            StartTime = startTime;
            EndTime = endTime;
            Texts = texts;
            CueSettingData = cueSettingData;
        }

        public string CueSettingData { get; set; }
        public string Number { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public IList<IText> Texts { get; set; }
    }
}
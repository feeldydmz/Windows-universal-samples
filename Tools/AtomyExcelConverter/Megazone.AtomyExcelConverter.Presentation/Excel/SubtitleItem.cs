using System;

namespace Megazone.AtomyExcelConverter.Presentation.Excel
{
    internal class SubtitleItem
    {
        public SubtitleItem(int number, TimeSpan startTime, TimeSpan endTime)
        {
            Number = number;
            StartTime = startTime;
            EndTime = endTime;
        }

        public int Number { get;}
        public TimeSpan StartTime { get;}
        public TimeSpan EndTime { get;}
        public string Text { get; set; }
    }
}

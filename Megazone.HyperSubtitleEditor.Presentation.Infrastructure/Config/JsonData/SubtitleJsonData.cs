namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData
{
    public class SubtitleJsonData
    {
        public int SingleLineMaxBytes { get; set; } = 21;
        public int MaxLines { get; set; } = 2;
        public int MaxCharactersPerSecond { get; set; } = 3;
        public double StartEndTimeTickMilliseconds { get; set; } = 100;
        public double MinDurationMilliseconds { get; set; } = 1000;
        public double MaxDurationMilliseconds { get; set; } = 7000;
        public double MinGapMilliseconds { get; set; } = 100;
        public int MediaBufferingSeconds { get; set; } = 5;
        public bool AutoLogin { get; set; } = false;
        public string Language { get; set; } = "";
    }
}
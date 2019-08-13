namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData
{
    public class GeneralJsonData
    {
        public bool IsAutoLogin { get; set; } = false;

        public int ConnectionTimeout { get; set; } = 100 * 1000;

        public int RetryCount { get; set; } = 3;

        public int RetryInterval { get; set; } = 0 * 1000;

        public bool IsRetry { get; set; } = true;

        public bool IsShowOnlyMediaFile { get; set; } = false;
    }
}
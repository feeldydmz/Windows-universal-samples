using System;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData
{
    public class DownloadJsonData
    {
        public DownloadJsonData()
        {
            ThreadCount = Environment.ProcessorCount;
        }

        public int ThreadCount { get; set; }

        public bool IsSecureProtocol { get; set; } = true;

        public long ChunkSize { get; set; } = 10 * 1024 * 1024;

        public bool AutoSuccessfullyCompletedQueueClear { get; set; } = false;

        public bool UseTransferAcceleration { get; set; } = false;
    }
}
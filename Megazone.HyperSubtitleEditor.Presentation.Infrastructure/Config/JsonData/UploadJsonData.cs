using System;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData
{
    public class UploadJsonData
    {
        public UploadJsonData()
        {
            ThreadCount = Environment.ProcessorCount * 2;
        }

        public int ThreadCount { get; set; }

        public bool IsSecureProtocol { get; set; } = true;

        public long ChunkSize { get; set; } = 10 * 1024 * 1024;

        public bool ShowSetting { get; set; } = true;

        public bool UseTransferAcceleration { get; set; } = false;
    }
}
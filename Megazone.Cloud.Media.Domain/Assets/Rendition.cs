using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class Rendition : IAssetElement
    {
        public Rendition(IEnumerable<AudioSetting> audioSettings, VideoSetting videoSetting, IEnumerable<string> urls, long duration, long size)
        {
            AudioSettings = audioSettings;
            VideoSetting = videoSetting;
            Urls = urls;
            Duration = duration;
            Size = size;
        }

        public IEnumerable<AudioSetting> AudioSettings { get; }
        public VideoSetting VideoSetting { get; }
        public IEnumerable<string> Urls { get; }
        public long Duration { get; }
        public long Size { get; }
    }
}

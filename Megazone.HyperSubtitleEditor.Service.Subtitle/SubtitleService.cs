using System;
using System.Collections.Generic;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class SubtitleService
    {
        public IList<ISubtitleItem> Load(string text, TrackFormat type)
        {
            return GetSubtitleParser(type).Parse(text);
        }

        public string ConvertToText(IEnumerable<string> subtitles, TrackFormat type)
        {
            return GetSubtitleParser(type).ToText(subtitles);
        }

        public ISubtitleParser GetSubtitleParser(TrackFormat type)
        {
            switch (type)
            {
                case TrackFormat.WebVtt:
                    return new WebVttParser();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
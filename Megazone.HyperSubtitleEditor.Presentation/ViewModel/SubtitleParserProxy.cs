using System;
using System.Collections.Generic;
using Megazone.Core.IoC;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.VideoTrack.WebVtt;
using Megazone.HyperSubtitleEditor.Presentation.Excel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class SubtitleParserProxy
    {
        public IEnumerable<SubtitleItem> Load(string text, TrackFormat type)
        {
            return GetSubtitleParser(type)
                .Parse(text);
        }

        public string ConvertToText(IEnumerable<string> subtitles, TrackFormat type)
        {
            return GetSubtitleParser(type)
                .ToText(subtitles);
        }

        public ISubtitleParser GetSubtitleParser(TrackFormat type)
        {
            switch (type)
            {
                case TrackFormat.WebVtt:
                    return new WebVttParser();
                case TrackFormat.Sami:
                    return new SamiParser();
                case TrackFormat.Srt:
                    return new SrtParser();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
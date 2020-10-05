using System;
using System.Collections.Generic;
using Megazone.Core.IoC;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.VideoTrack.WebVtt;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class SubtitleParserProxy
    {
        public IEnumerable<SubtitleItem> Load(string text, SubtitleFormatKind format)
        {
            return GetSubtitleParser(format)
                .Parse(text);
        }

        //TODO �� �Լ��� webVtt ���� �����, webVtt�� SubtitleItem �� ��� �ϴ� ������� �ٲٰ� �� �Լ��� �����ؾ� ��
        public string ConvertToText(IEnumerable<string> subtitles, SubtitleFormatKind type)
        {
            return GetSubtitleParser(type)
                .ToText(subtitles);
        }

        public string ConvertToText(IEnumerable<SubtitleItem> subtitleItems, SubtitleFormatKind type)
        {
            return GetSubtitleParser(type)
                .ToText(subtitleItems);
        }

        public ISubtitleParser GetSubtitleParser(SubtitleFormatKind type)
        {
            switch (type)
            {
                case SubtitleFormatKind.WebVtt:
                    return new WebVttParser();
                case SubtitleFormatKind.Sami:
                    return new SamiParser();
                case SubtitleFormatKind.Srt:
                    return new SrtParser();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
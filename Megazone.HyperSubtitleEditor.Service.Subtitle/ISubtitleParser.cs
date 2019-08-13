using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    internal interface ISubtitleParser
    {
        IList<ISubtitleItem> Parse(string text);

        string ToText(IEnumerable<string> subtitles);
    }
}
using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    public interface ISubtitleService
    {
        IList<ISubtitleItem> Load(string text, TrackFormat type);
        string ConvertToText(IEnumerable<string> subtitles, TrackFormat type);
    }
}
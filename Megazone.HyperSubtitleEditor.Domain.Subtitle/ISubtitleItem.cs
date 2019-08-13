using System;
using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle
{
    public interface ISubtitleItem
    {
        string Number { get; }

        TimeSpan StartTime { get; }

        TimeSpan EndTime { get; }

        IList<IText> Texts { get; }
    }
}
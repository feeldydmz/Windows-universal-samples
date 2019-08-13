using System;

namespace Megazone.HyperSubtitleEditor.Presentation
{
    public interface ITimeSeeker
    {
        Action<decimal> Seek { get; set; }
        Action SeekComplatedAction { get; }
    }
}
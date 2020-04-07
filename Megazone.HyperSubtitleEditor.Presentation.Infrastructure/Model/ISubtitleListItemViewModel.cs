using System;
using System.Collections.Generic;
using Megazone.Core.VideoTrack;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model
{
    public interface ISubtitleListItemViewModel
    {
        int Number { get; set; }
        TimeSpan StartTime { get; set; }
        TimeSpan EndTime { get; set; }
        TimeSpan Duration { get; set; }
        bool IsNowPlaying { get; set; }
        IList<IText> Texts { get; set; }
        bool IsStartTimeValid { get; set; }
        bool IsEndTimeValid { get; set; }
        string DisplayText { get; set; }
        bool IsSelected { get; set; }
        bool IsOverMaxCharacterPerSecond { get; set; }
        double TextCharsPerSecond { get; }
        int TextTotalLength { get; }
        bool IsDurationValid { get; set; }
        bool IsDirty();
        void CheckIsNowPlaying(TimeSpan position);
        void ResetDirtyCheckFlags();

        void ResetData();
    }
}
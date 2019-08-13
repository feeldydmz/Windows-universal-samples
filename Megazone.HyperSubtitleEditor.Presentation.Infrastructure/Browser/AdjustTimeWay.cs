using System;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser
{
    public class AdjustTimeWay
    {
        public AdjustTimeWay(TimeSpan time, AdjustTimeRange range, AdjustTimeBehavior behavior)
        {
            Time = time;
            Range = range;
            Behavior = behavior;
        }

        public TimeSpan Time { get; }
        public AdjustTimeRange Range { get; }
        public AdjustTimeBehavior Behavior { get; }
    }
}
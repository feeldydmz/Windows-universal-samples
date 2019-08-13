using System;

namespace Megazone.HyperSubtitleEditor.Presentation
{
    public class MediaTimeSeeker : ITimeSeeker
    {
        private Action _seekComplatedAction;
        Action<decimal> ITimeSeeker.Seek { get; set; }
        Action ITimeSeeker.SeekComplatedAction => _seekComplatedAction;

        public void Run(decimal seekPositionSeconds, Action complatedAction = null)
        {
            (this as ITimeSeeker).Seek?.Invoke(seekPositionSeconds);
            _seekComplatedAction = complatedAction;
        }

        public void Run(TimeSpan seekPosition, Action complatedAction = null)
        {
            (this as ITimeSeeker).Seek?.Invoke(Convert.ToDecimal(seekPosition.TotalSeconds));
            _seekComplatedAction = complatedAction;
        }
    }
}
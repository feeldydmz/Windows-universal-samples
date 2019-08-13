using System.Collections.Generic;
using System.Linq;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class SubtitleListItemValidator
    {
        public bool IsEnabled { get; set; } = true;

        public void Validate(IList<ISubtitleListItemViewModel> items)
        {
            if (!IsEnabled) return;
            if (items == null || !items.Any()) return;
            var itemsCount = items.Count;
            for (var i = 0; i < itemsCount; i++)
            {
                var currentItem = items[i];
                var previousItem = i > 0 ? items[i - 1] : null;
                var currentDuration = currentItem.Duration;
                var maxDuration = ConfigHolder.Current.Subtitle.MaxDuration;
                var minDuration = ConfigHolder.Current.Subtitle.MinDuration;
                currentItem.IsDurationValid = currentDuration >= minDuration && currentDuration <= maxDuration;
                var isStartTimeValid = true;
                var isMinGapValid = true;
                if (previousItem != null)
                {
                    isStartTimeValid = previousItem.EndTime <= currentItem.StartTime;
                    var minGap = ConfigHolder.Current.Subtitle.MinGap;
                    isMinGapValid = currentItem.StartTime - previousItem.EndTime >= minGap;
                    previousItem.IsEndTimeValid = isStartTimeValid && isMinGapValid;
                }
                currentItem.IsStartTimeValid = isStartTimeValid && isMinGapValid;
            }
        }
    }
}
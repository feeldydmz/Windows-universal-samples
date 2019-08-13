using System;
using Megazone.HyperSubtitleEditor.Presentation.Excel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal struct SubtitleListItemParserProvider
    {
        public static ISubtitleListItemViewModelParser Get(TrackFormat type)
        {
            switch (type)
            {
                case TrackFormat.WebVtt:
                    return new SubtitleListItemViewModelToWebVttStringParaser();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
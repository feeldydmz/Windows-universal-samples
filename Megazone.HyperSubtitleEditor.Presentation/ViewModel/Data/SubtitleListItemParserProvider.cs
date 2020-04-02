using System;
using Megazone.Core.VideoTrack;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal struct SubtitleListItemParserProvider
    {
        public static ISubtitleListItemViewModelParser Get(SubtitleFormatKind type)
        {
            switch (type)
            {
                case SubtitleFormatKind.WebVtt:
                    return new SubtitleListItemViewModelToWebVttStringParaser();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
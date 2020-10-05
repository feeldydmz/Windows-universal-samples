using Megazone.Core.VideoTrack.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal static class SubtitleListItemViewModelExtension
    {
        public static string ConvertToString(this ISubtitleListItemViewModel item,
            ISubtitleListItemViewModelParser parser)
        {
            return parser.Run(item);
        }

        public static SubtitleItem ConvertToSubtitleItem(this ISubtitleListItemViewModel item)
        {
            var newItem = new SubtitleItem
            {
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                Number = item.Number,
                Texts = item.Texts
            };


            return newItem;
        }
    }
}
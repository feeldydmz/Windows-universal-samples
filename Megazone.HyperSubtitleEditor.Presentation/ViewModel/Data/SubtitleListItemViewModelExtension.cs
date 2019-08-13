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
    }
}
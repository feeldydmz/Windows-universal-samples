using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal interface ISubtitleListItemViewModelParser
    {
        string Run(ISubtitleListItemViewModel item);
    }
}
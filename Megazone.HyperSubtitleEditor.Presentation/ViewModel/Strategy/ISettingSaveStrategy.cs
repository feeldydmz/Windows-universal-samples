using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Strategy
{
    internal interface ISettingSaveStrategy
    {
        void Save(ConfigHolder config, bool isSilence = false);
    }
}
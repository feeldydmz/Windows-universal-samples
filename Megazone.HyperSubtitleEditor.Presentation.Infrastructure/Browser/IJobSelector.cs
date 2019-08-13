using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser
{
    public interface IJobSelector
    {
        ILoadingManager LoadingManager { get; }
        void Show();
    }
}
using System.Windows;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser
{
    public interface IBrowser
    {
        IMainView Main { get; }
        MessageBoxResult ShowConfirmWindow(ConfirmWindowParameter parameter);
    }
}
using System.Windows;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation
{
    [Inject(Source = typeof(IBrowser), Scope = LifetimeScope.Singleton)]
    internal class Browser : IBrowser
    {
        public IMainView Main => MainView.Self;

        public MessageBoxResult ShowConfirmWindow(ConfirmWindowParameter parameter)
        {
            return ConfirmWindow.ShowDialog(parameter);
        }
    }
}
using System.Windows;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.Browser;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.View;
using Megazone.AtomyExcelConverter.Presentation.View;
using Megazone.Core.IoC;

namespace Megazone.AtomyExcelConverter.Presentation
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

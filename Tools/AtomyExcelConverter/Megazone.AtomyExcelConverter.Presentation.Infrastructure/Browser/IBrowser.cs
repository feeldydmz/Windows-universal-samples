using System.Windows;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.View;

namespace Megazone.AtomyExcelConverter.Presentation.Infrastructure.Browser
{
    public interface IBrowser
    {
        IMainView Main { get; }
        MessageBoxResult ShowConfirmWindow(ConfirmWindowParameter parameter);
    }
}

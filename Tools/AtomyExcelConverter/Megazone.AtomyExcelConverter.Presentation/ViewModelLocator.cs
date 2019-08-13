using System.ComponentModel;
using System.Windows;
using Megazone.AtomyExcelConverter.Presentation.ViewModel;
using Microsoft.Practices.Unity;

namespace Megazone.AtomyExcelConverter.Presentation
{
    internal class ViewModelLocator
    {
        public ViewModelLocator()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            new Bootstrapper().Initialize();
        }

        public MainViewModel Main => Bootstrapper.Container.Resolve<MainViewModel>();
    }
}

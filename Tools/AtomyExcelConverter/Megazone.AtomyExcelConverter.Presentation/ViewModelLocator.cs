using System.ComponentModel;
using System.Windows;
using Megazone.AtomyExcelConverter.Presentation.ViewModel;
using Unity;

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

using System.ComponentModel;
using System.Windows;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation
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
        public SubtitleViewModel Subtitle => Bootstrapper.Container.Resolve<SubtitleViewModel>();
        public SettingViewModel Setting => Bootstrapper.Container.Resolve<SettingViewModel>();
        public ImportExcelViewModel ImportExcel => Bootstrapper.Container.Resolve<ImportExcelViewModel>();

        public JobMediaItemSelectorViewModel JobMediaItemSelector =>
            Bootstrapper.Container.Resolve<JobMediaItemSelectorViewModel>();

        public JobSelectorViewModel JobSelector => Bootstrapper.Container.Resolve<JobSelectorViewModel>();
        public OpenSubtitleViewModel OpenSubtitle => Bootstrapper.Container.Resolve<OpenSubtitleViewModel>();

        public AddAndEditSubtitleViewModel AddAndEditSubtitle =>
            Bootstrapper.Container.Resolve<AddAndEditSubtitleViewModel>();

        public CopySubtitleViewModel CopySubtitle => Bootstrapper.Container.Resolve<CopySubtitleViewModel>();
        public GoToLineViewModel GoToLine => Bootstrapper.Container.Resolve<GoToLineViewModel>();
        public FindAndReplaceViewModel FindAndReplace => Bootstrapper.Container.Resolve<FindAndReplaceViewModel>();
    }
}
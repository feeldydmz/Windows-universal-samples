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
        public OpenSubtitleViewModel OpenSubtitle => Bootstrapper.Container.Resolve<OpenSubtitleViewModel>();

        public AddAndEditSubtitleViewModel AddAndEditSubtitle =>
            Bootstrapper.Container.Resolve<AddAndEditSubtitleViewModel>();

        public CopySubtitleViewModel CopySubtitle => Bootstrapper.Container.Resolve<CopySubtitleViewModel>();
        public GoToLineViewModel GoToLine => Bootstrapper.Container.Resolve<GoToLineViewModel>();
        public FindAndReplaceViewModel FindAndReplace => Bootstrapper.Container.Resolve<FindAndReplaceViewModel>();
        public SignInViewModel SignIn => Bootstrapper.Container.Resolve<SignInViewModel>();

        public CaptionAssetMenuViewModel CaptionAssetMenu =>
            Bootstrapper.Container.Resolve<CaptionAssetMenuViewModel>();

        public VideoListViewModel VideoList => Bootstrapper.Container.Resolve<VideoListViewModel>();
        public McmDeployViewModel McmDeploy => Bootstrapper.Container.Resolve<McmDeployViewModel>();
        public ProjectViewModel McmProjectViewModel => Bootstrapper.Container.Resolve<ProjectViewModel>();
        public WorkBarViewModel WorkBar => Bootstrapper.Container.Resolve<WorkBarViewModel>();
        public LeftSideMenuViewModel LeftSideMenu => Bootstrapper.Container.Resolve<LeftSideMenuViewModel>();
        public OpenVideoViewModel OpenVideoAsset => Bootstrapper.Container.Resolve<OpenVideoViewModel>();
        public AssetEditorViewModel AssetEditor => Bootstrapper.Container.Resolve<AssetEditorViewModel>();
        public MetadataViewModel MetaData => Bootstrapper.Container.Resolve<MetadataViewModel>();

        public CaptionElementsEditViewModel CaptionElementEdit =>
            Bootstrapper.Container.Resolve<CaptionElementsEditViewModel>();
        public CaptionAssetListViewModel CaptionAssetList =>
            Bootstrapper.Container.Resolve<CaptionAssetListViewModel>();

    }
}
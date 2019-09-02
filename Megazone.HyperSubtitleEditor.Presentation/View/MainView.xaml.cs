using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl, IMainView
    {
        private ILoadingManager _loadingManager;

        public MainView()
        {
            Self = this;
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        internal static IMainView Self { get; private set; }

        public ILoadingManager LoadingManager
        {
            get { return _loadingManager = _loadingManager ?? new LoadingManager(LoadingProgressView); }
        }

        public IJobSelector JobSelector { get; } = new JobSelectorImpl();
        public IJobMediaItemSelector JobMediaItemSelector { get; } = new JobMediaItemSelectorImpl();

        public void ShowSettingsDialog()
        {
            var view = new SettingView();
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_SETTING, // TODO: 다국어
                //SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public void ShowImportExcelDialog(string initialFilePath = null)
        {
            var view = new ImportExcelView();
            var vm = (ImportExcelViewModel) view.DataContext;
            vm.InitialFilePath = initialFilePath;
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_IMPORT_EXCEL, // TODO: 다국어
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public void ShowOpenSubtitleDialog(string initialFilePath = null)
        {
            var view = new OpenSubtitleView();
            var vm = (OpenSubtitleViewModel) view.DataContext;
            vm.InitialFilePath = initialFilePath;
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_OPEN_CAPTION_FILE, // TODO: 다국어
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public void ShowAddAndEditSubtitleDialog(SubtitleDialogViewMode viewMode, ISubtitleTabItemViewModel tabItem,
            string title)
        {
            var view = new AddAndEditSubtitleView();
            var viewModel = (AddAndEditSubtitleViewModel) view.DataContext;
            viewModel.Mode = viewMode;

            if (tabItem != null)
                viewModel.SetSelectedTabInfo(tabItem);
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = title, // TODO: 다국어
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public void ShowCopySubtitleDialog(IEnumerable<ISubtitleTabItemViewModel> tabs)
        {
            var view = new CopySubtitleView();
            var viewModel = (CopySubtitleViewModel) view.DataContext;
            viewModel.SetSelectedTabInfo(tabs);

            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_COPY_CAPTION, 
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public ISubtitleView SubtitleView { get; } = new SubtitleViewImpl();

        public void ShowFindDialog()
        {
            var view = new FindAndReplaceView
            {
                FindModeButton =
                {
                    IsChecked = true
                }
            };
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_FIND, // TODO: 다국어
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
        }

        public void ShowFindAndReplaceDialog()
        {
            var view = new FindAndReplaceView
            {
                ReplaceModeButton =
                {
                    IsChecked = true
                }
            };
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_REPLACE, // TODO: 다국어
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
        }

        public AdjustTimeWay ShowAdjustTimeWindow()
        {
            var wnd = new AdjustTimeWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            wnd.ShowDialog();
            return new AdjustTimeWay(wnd.Time, wnd.Range, wnd.Behavior);
        }

        public void ShowOpenVideoAddressWindow()
        {
            var wnd = new OpenVideoAddressWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            wnd.ShowDialog();
        }

        public void ShowApplicationInfoWindow()
        {
            var wnd = new ApplicationInformationWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            wnd.ShowDialog();
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            RootViewContainer.Child = new SubtitleView();
        }

        public void ShowVideoListDialog()
        {
            var view = new VideoListView();
            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 600,
                Height = 450,
                Content = view,
                Title = Resource.CNT_VIDEO
            };
            if (view.DataContext is VideoListViewModel viewModel)
            {
                viewModel.SetTitleAction = (title) => { wnd.Title = title; };
            }
            wnd.ShowDialog();
        }

        public void ShowMcmDeployDialog()
        {
            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_PUBLISH, // TODO: 다국어
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 600,
                Height = 450,
                Content = new McmDeployView()
            };
            wnd.ShowDialog();
        }
        public void ShowMcmDeployConfirmDialog(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions, string linkUrl)
        {
            var captionItems = captions?.Select(caption => new CaptionElementItemViewModel(caption)).ToList();
            var view = new McmDeployConfirmView
            {
                DataContext = new McmDeployConfirmViewModel()
                {
                    VideoItem = new VideoItemViewModel(video),
                    CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset),
                    CaptionItems = captionItems,
                    Url = linkUrl
                }
            };
            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_PUBLISH_CONFIRM, // TODO: 다국어
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 514,
                Height = 354,
                Content = view
            };
            wnd.ShowDialog();
        }

        public void SetWindowTitle(string title)
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Title = title;
        }

        private class SubtitleViewImpl : ISubtitleView
        {
            public void ShowGoToLineDialog(int maximumNumber)
            {
                var goToLineView = new GoToLineView();
                var vm = (GoToLineViewModel) goToLineView.DataContext;
                vm.MaximumNumber = maximumNumber;
                var window = new ChildWindow
                {
                    Owner = Application.Current.MainWindow,
                    Title = Resource.CNT_GO_TO_CAPTION_NUMBER, // TODO: 다국어
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1200,
                    Height = 900,
                    Content = goToLineView,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                window.ShowDialog();
            }
        }

        private class JobMediaItemSelectorImpl : IJobMediaItemSelector
        {
            private JobMediaItemSelectorView _jobMediaItemSelectorView;

            public ILoadingManager LoadingManager => _jobMediaItemSelectorView?.LoadingManager;

            public void Show()
            {
                _jobMediaItemSelectorView = new JobMediaItemSelectorView();
                var window = new ChildWindow
                {
                    Owner = Application.Current.MainWindow,
                    Title = Resource.CNT_SELECT_VIDEO, // TODO: 다국어
                    ResizeMode = ResizeMode.CanResize,
                    Width = 320,
                    MinWidth = 320,
                    Height = 440,
                    MinHeight = 440,
                    SizeToContent = SizeToContent.Height,
                    Content = _jobMediaItemSelectorView,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                window.ShowDialog();
                window.Closed += Window_Closed;
            }

            private void Window_Closed(object sender, EventArgs e)
            {
                if (sender is Window window)
                    window.Closed -= Window_Closed;

                _jobMediaItemSelectorView = null;
            }
        }

        private class JobSelectorImpl : IJobSelector
        {
            private JobSelectorView _jobSelectorView;

            public ILoadingManager LoadingManager => _jobSelectorView?.LoadingManager;

            public void Show()
            {
                _jobSelectorView = new JobSelectorView();
                var window = new ChildWindow
                {
                    Owner = Application.Current.MainWindow,
                    Title = Resource.CNT_SELECT_JOB, // TODO: 다국어
                    ResizeMode = ResizeMode.NoResize,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Content = _jobSelectorView,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                window.ShowDialog();
                window.Closed += Window_Closed;
            }

            private void Window_Closed(object sender, EventArgs e)
            {
                if (sender is Window window)
                    window.Closed -= Window_Closed;

                _jobSelectorView = null;
            }
        }
    }
}
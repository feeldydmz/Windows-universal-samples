using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.Extension;
using Megazone.Core.VideoTrack;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Microsoft.Win32;
using Unity;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;

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
            CheckExplorerVersionRegistry();
            Loaded += MainView_Loaded;
        }

        internal static IMainView Self { get; private set; }

        public ILoadingManager LoadingManager
        {
            get { return _loadingManager = _loadingManager ?? new LoadingManager(LoadingProgressView); }
        }

        //public IJobSelector JobSelector { get; } = new JobSelectorImpl();
        //public IJobMediaItemSelector JobMediaItemSelector { get; } = new JobMediaItemSelectorImpl();

        public void ShowSettingsDialog()
        {
            var view = new SettingView();
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_SETTING,
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
                Title = Resource.CNT_IMPORT_EXCEL,
                ResizeMode = ResizeMode.NoResize,
                Width = 500,
                Height = 600,
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public void ShowOpenSubtitleDialog(string filePath, SubtitleFormatKind subtitleFormat)
        {
            var view = new OpenSubtitleView();
            var vm = (OpenSubtitleViewModel) view.DataContext;
            vm.FilePath = filePath;
            vm.SubtitleFormat = subtitleFormat;
            var window = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_OPEN_CAPTION_FILE,
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
                Title = Resource.CNT_FIND,
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
                Title = Resource.CNT_REPLACE,
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

        public bool? ShowCreateWorkspaceConfirmWindow()
        {
            var wnd = new CreateWorksapceConfirmView
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            wnd.ShowDialog();

            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                return wnd.IsCreateNewWindow;
            }
            else
            {
                return null;
            }
        }

        public void ShowOpenVideoAddressWindow(string openTypeString)
        {
            var wnd = new OpenVideoAddressWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                OpenTypeString = openTypeString
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
                viewModel.SetTitleAction = title => { wnd.Title = title; };
            wnd.ShowDialog();
        }

        public void ShowMcmDeployAndAssetCreateDialog()
        {
            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_SAVE,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 600,
                Height = 450,
                Content = new McmDeployAndAssetCreateView()
            };
            wnd.ShowDialog();
        }

        public void ShowMcmDeployDialog()
        {
            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_SAVE,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 600,
                Height = 450,
                Content = new McmDeployView()
            };
            wnd.ShowDialog();
        }

        public void ShowMcmDeployConfirmDialog(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions,
            string linkUrl)
        {
            var viewModel = new McmDeployConfirmViewModel();
            viewModel.Update(video, captionAsset, captions, linkUrl);

            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_CONFIRM,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 514,
                Height = 354,
                Content = new McmDeployConfirmView
                {
                    DataContext = viewModel
                }
            };
            wnd.ShowDialog();
        }

        public void SetWindowTitle(string title)
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Title = title;
        }

        public void ShowAssetEditorDialog(CaptionAsset captionAsset = null)
        {
            var view = new AssetEditorView
            {
                DataContext = new AssetEditorViewModel
                {
                    CaptionAsset = captionAsset
                }
            };

            var wnd = new ChildWindow
            {
                Owner = Application.Current.MainWindow,
                Title = Resource.CNT_ASSET_NAME_EDIT,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 514,
                Height = 200,
                Content = view
            };
            wnd.ShowDialog();
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            RootViewContainer.Child = new SubtitleView();
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        private void CheckExplorerVersionRegistry()
        {
            try
            {
                string regSearchKey;

                var is64BitProcess = Environment.Is64BitOperatingSystem;

                if (is64BitProcess)
                {
                    regSearchKey =
                        "SOFTWARE\\WOW6432Node\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
                }
                else
                {
                    regSearchKey =
                        "SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
                }

                // 서브키를 얻어온다. 없으면 null
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(regSearchKey, false);
                // 없으면 서브키를 만든다.
                var processName = System.AppDomain.CurrentDomain.FriendlyName;
                if (rk != null)
                {
                    var returnValue = rk.GetValue(processName);

                    if (returnValue == null)
                    {
                        CreateExplorerVersionRegistry(regSearchKey, processName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        private void CreateExplorerVersionRegistry(string keyPath, string processName)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "reg.exe";
                    proc.StartInfo.UseShellExecute = true;
                    //proc.StartInfo.RedirectStandardOutput = true;
                    //proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.Arguments =
                        $"add \"HKEY_LOCAL_MACHINE\\{keyPath}\" /v {processName} /t REG_DWORD /d 10001 /f";
                    proc.StartInfo.Verb = "runas";
                    proc.Start();
                    //string stdout = proc.StandardOutput.ReadToEnd();
                    //string stderr = proc.StandardError.ReadToEnd();
                    //proc.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        public void RestartMainWindow()
        {
            var browser = Bootstrapper.Container.Resolve<IBrowser>();
            var subtitle = Bootstrapper.Container.Resolve<SubtitleViewModel>();

            if (subtitle.CheckWorkInProgress())
            {
                if (browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                        Resource.MSG_PRGRAM_ENDS_IN_PROGRESS,
                        MessageBoxButton.OKCancel,
                        Application.Current.MainWindow,
                        TextAlignment.Center)) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing -= MainWindow_Closing;

                var applicationPath = this.StartUpPath() + this.GetApplicationName();

                // 강제 종료시키고, 재실행하도록 한다.
                if (Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Close();
                Process.Start(applicationPath, "-r");
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var browser = Bootstrapper.Container.Resolve<IBrowser>();
            var subtitle = Bootstrapper.Container.Resolve<SubtitleViewModel>();

            if (subtitle.CheckWorkInProgress())
            {
                if (browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO, Resource.MSG_PRGRAM_ENDS_IN_PROGRESS,
                        MessageBoxButton.OKCancel,
                        Application.Current.MainWindow,
                        TextAlignment.Center)) == MessageBoxResult.Cancel)
                    e.Cancel = true;
            }

            subtitle.Unload();
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
                    Title = Resource.CNT_GO_TO_CAPTION_NUMBER,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1200,
                    Height = 900,
                    Content = goToLineView,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                window.ShowDialog();
            }
        }

        //private class JobMediaItemSelectorImpl : IJobMediaItemSelector
        //{
        //    private JobMediaItemSelectorView _jobMediaItemSelectorView;

        //    public ILoadingManager LoadingManager => _jobMediaItemSelectorView?.LoadingManager;

        //    public void Show()
        //    {
        //        _jobMediaItemSelectorView = new JobMediaItemSelectorView();
        //        var window = new ChildWindow
        //        {
        //            Owner = Application.Current.MainWindow,
        //            Title = Resource.CNT_SELECT_VIDEO, // TODO: 다국어
        //            ResizeMode = ResizeMode.CanResize,
        //            Width = 320,
        //            MinWidth = 320,
        //            Height = 440,
        //            MinHeight = 440,
        //            SizeToContent = SizeToContent.Height,
        //            Content = _jobMediaItemSelectorView,
        //            WindowStartupLocation = WindowStartupLocation.CenterOwner
        //        };
        //        window.ShowDialog();
        //        window.Closed += Window_Closed;
        //    }

        //    private void Window_Closed(object sender, EventArgs e)
        //    {
        //        if (sender is Window window)
        //            window.Closed -= Window_Closed;

        //        _jobMediaItemSelectorView = null;
        //    }
        //}

        //private class JobSelectorImpl : IJobSelector
        //{
        //    private JobSelectorView _jobSelectorView;

        //    public ILoadingManager LoadingManager => _jobSelectorView?.LoadingManager;

        //    public void Show()
        //    {
        //        _jobSelectorView = new JobSelectorView();
        //        var window = new ChildWindow
        //        {
        //            Owner = Application.Current.MainWindow,
        //            Title = Resource.CNT_SELECT_JOB, // TODO: 다국어
        //            ResizeMode = ResizeMode.NoResize,
        //            SizeToContent = SizeToContent.WidthAndHeight,
        //            Content = _jobSelectorView,
        //            WindowStartupLocation = WindowStartupLocation.CenterOwner
        //        };
        //        window.ShowDialog();
        //        window.Closed += Window_Closed;
        //    }

        //    private void Window_Closed(object sender, EventArgs e)
        //    {
        //        if (sender is Window window)
        //            window.Closed -= Window_Closed;

        //        _jobSelectorView = null;
        //    }
        //}
    }
}
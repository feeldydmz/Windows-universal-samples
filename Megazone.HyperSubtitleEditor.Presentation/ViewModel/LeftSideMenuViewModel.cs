using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class LeftSideMenuViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly RecentlyLoader _recentlyLoader;
        private readonly VideoListViewModel _videoList;
        private ICommand _closeCommand;
        private bool _hasRegisteredMessageHandlers;
        private bool _isOpen;
        private ICommand _loadCommand;

        private ICommand _loadRecentlyCommand;

        private ICommand _openRecentlyCommand;

        private List<RecentlyItem> _recentlyItems;

        private ICommand _RefreshCommand;
        private ICommand _unloadCommand;

        public LeftSideMenuViewModel(IBrowser browser, RecentlyLoader recentlyLoader, VideoListViewModel videoList)
        {
            _browser = browser;
            _recentlyLoader = recentlyLoader;
            _videoList = videoList;
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => Set(ref _isOpen, value);
        }

        public List<RecentlyItem> RecentlyItems
        {
            get => _recentlyItems;
            set => Set(ref _recentlyItems, value);
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand = _closeCommand ?? new RelayCommand(Close); }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand UnloadCommand
        {
            get { return _unloadCommand = _unloadCommand ?? new RelayCommand(Unload); }
        }

        public ICommand LoadRecentlyCommand
        {
            get { return _loadRecentlyCommand = _loadRecentlyCommand ?? new RelayCommand(LoadRecently); }
        }

        public ICommand OpenRecentlyCommand
        {
            get { return _openRecentlyCommand = _openRecentlyCommand ?? new RelayCommand<RecentlyItem>(OpenRecently); }
        }

        public ICommand RefreshCommand
        {
            get { return _RefreshCommand = _RefreshCommand ?? new RelayCommand(Refresh); }
        }

        private void Refresh()
        {
            var videoListViewModel = Bootstrapper.Container.Resolve<VideoListViewModel>();
            var captionAssetListViewModel = Bootstrapper.Container.Resolve<CaptionAssetListViewModel>();

            videoListViewModel.ClearSearchParameter();
            videoListViewModel.LoadCommand?.Execute(null);

            captionAssetListViewModel.ClearSearchParameter();
            captionAssetListViewModel.LoadCommand?.Execute(null);
        }

        private void OpenRecently(RecentlyItem recentlyItem)
        {
            var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            if (subtitleVm.Tabs?.Any() ?? false)
            {
                if (subtitleVm.Tabs.Any(tab => tab.CheckDirty()))
                    if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                            Resource.MSG_THERE_IS_WORK_IN_PROGRESS, 
                            MessageBoxButton.OKCancel,
                            Application.Current.MainWindow)) !=
                        MessageBoxResult.OK)
                        return;

                var removeTabs = subtitleVm.Tabs.ToList();
                foreach (var tab in removeTabs)
                    MessageCenter.Instance.Send(
                        new Message.SubtitleEditor.CloseTabMessage(this, tab as SubtitleTabItemViewModel));
            }

            // 선택된 video 정보를 메인 
            var video = recentlyItem.Video;
            var asset = recentlyItem.CaptionAsset;
            var localFileFullPath = recentlyItem.LocalFileFullPath;
            var selectedCaptionList = recentlyItem.Captions?.ToList() ?? new List<Caption>();

            
            if (localFileFullPath.IsNullOrEmpty())
            {
                MessageCenter.Instance.Send(new CloudMedia.CaptionOpenRequestedMessage(this,
                    new CaptionOpenMessageParameter(video, asset, selectedCaptionList, true)));
            }
            else
            {
                //var fileExtension = Path.GetExtension(localFileFullPath);

                var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();

                subtitleViewModel.ImportSubtitleFile(localFileFullPath);

                //if (fileExtension == ".xlsx")
                //    _browser.Main.ShowImportExcelDialog(localFileFullPath);
                //else
                //    _browser.Main.ShowOpenSubtitleDialog(localFileFullPath);
            }

            MessageCenter.Instance.Send(new LeftSideMenu.CloseMessage(this));
        }

        private void LoadRecently()
        {
            _recentlyLoader.Load();
            var tempList = _recentlyLoader.GetRecentlyItems().ToList();

            var removeItems = new List<RecentlyItem>();

            foreach (var recentlyItem in tempList)
                if (recentlyItem.FirstName.IsNullOrEmpty() && recentlyItem.SecondName.IsNullOrEmpty())
                    removeItems.Add(recentlyItem);

            foreach (var removeItem in removeItems) tempList.Remove(removeItem);

            RecentlyItems = tempList;
        }

        private void Load()
        {
            RegisterMessageHandlers();
        }

        private void Unload()
        {   
            Console.WriteLine("Unload");
            //UnregisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            if (_hasRegisteredMessageHandlers) return;
            _hasRegisteredMessageHandlers = true;

            MessageCenter.Instance.Regist<LeftSideMenu.OpenMessage>(OpenLeftSideMenuRequest);
            MessageCenter.Instance.Regist<LeftSideMenu.CloseMessage>(CloseLeftSideMenuRequest);
        }

        private void UnregisterMessageHandlers()
        {
            if (!_hasRegisteredMessageHandlers)
                return;

            MessageCenter.Instance.Unregist<LeftSideMenu.OpenMessage>(OpenLeftSideMenuRequest);
            MessageCenter.Instance.Unregist<LeftSideMenu.CloseMessage>(CloseLeftSideMenuRequest);
        }

        private void CloseLeftSideMenuRequest(LeftSideMenu.CloseMessage message)
        {
            Close();
        }

        private void OpenLeftSideMenuRequest(LeftSideMenu.OpenMessage message)
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
            _videoList.Close();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain.Assets;
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
        private readonly RecentlyLoader _recentlyLoader;
        private readonly IBrowser _browser;
        private ICommand _closeCommand;
        private bool _hasRegisteredMessageHandlers;
        private bool _isOpen;
        private ICommand _loadCommand;

        private ICommand _loadRecentlyCommand;

        private IEnumerable<RecentlyItem> _recentlyItems;
        private ICommand _unloadCommand;

        public LeftSideMenuViewModel(IBrowser browser, RecentlyLoader recentlyLoader)
        {
            _browser = browser;
            _recentlyLoader = recentlyLoader;
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => Set(ref _isOpen, value);
        }

        public IEnumerable<RecentlyItem> RecentlyItems
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

        private ICommand _openRecentlyCommand;
        public ICommand OpenRecentlyCommand
        {
            get { return _openRecentlyCommand = _openRecentlyCommand ?? new RelayCommand<RecentlyItem>(OpenRecently); }
        }

        private void OpenRecently(RecentlyItem recentlyItem)
        {
            var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            if (subtitleVm.Tabs?.Any() ?? false)
            {
                if (subtitleVm.Tabs.Any(tab => tab.CheckDirty()))
                    // [resource]
                    if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                            "편집 내용이 있습니다. 열려진 탭을 모두 닫고, 선택된 자막으로 오픈됩니다.\n계속 진행하시겠습니까?", MessageBoxButton.OKCancel)) !=
                        MessageBoxResult.OK)
                        return;

                var removeTabs = subtitleVm.Tabs.ToList();
                foreach (var tab in removeTabs)
                    MessageCenter.Instance.Send(
                        new Subtitle.CloseTabMessage(this, tab as SubtitleTabItemViewModel));
            }

            // 선택된 video 정보를 메인 
            var video = recentlyItem.Video;
            var asset = recentlyItem.CaptionAsset;
            var selectedCaptionList = recentlyItem.Captions?.ToList() ?? new List<Caption>();

            MessageCenter.Instance.Send(new CloudMedia.CaptionOpenMessage(this,
                new CaptionOpenMessageParameter(video, asset, selectedCaptionList)));
            
            MessageCenter.Instance.Send(new LeftSideMenu.CloseMessage(this));
        }

        private void LoadRecently()
        {
            _recentlyLoader.Load();
            RecentlyItems = _recentlyLoader.GetRecentlyItems().ToList();
        }

        private void Load()
        {
            RegisterMessageHandlers();
        }

        private void Unload()
        {
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
        }
    }
}
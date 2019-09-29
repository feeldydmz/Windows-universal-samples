using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class LeftSideMenuViewModel : ViewModelBase
    {
        private readonly RecentlyLoader _recentlyLoader;
        private ICommand _closeCommand;
        private bool _hasRegisteredMessageHandlers;
        private bool _isOpen;
        private ICommand _loadCommand;

        private ICommand _loadRecentlyCommand;

        private IEnumerable<RecentlyItem> _recentlyItems;
        private ICommand _unloadCommand;

        public LeftSideMenuViewModel(RecentlyLoader recentlyLoader)
        {
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
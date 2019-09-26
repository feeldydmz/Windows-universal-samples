using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class LeftSideMenuViewModel : ViewModelBase
    {
        private ICommand _closeCommand;
        private ICommand _loadCommand;
        private ICommand _unloadCommand;
        private bool _isOpen;
        private bool _hasRegisteredMessageHandlers;

        public bool IsOpen
        {
            get => _isOpen;
            set => Set(ref _isOpen, value);
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
        }

        private void UnregisterMessageHandlers()
        {
            if (!_hasRegisteredMessageHandlers)
                return;

            MessageCenter.Instance.Unregist<LeftSideMenu.OpenMessage>(OpenLeftSideMenuRequest);
        }
        private void OpenLeftSideMenuRequest(LeftSideMenu.OpenMessage message)
        {
            IsOpen = true;
        }

        private void Close()
        {
            IsOpen = false;
        }
    }
}

using System;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    public class AssetEditorViewModel : ViewModelBase
    {
        private string _assetName;

        private ICommand _confirmCommand;
        private bool _isBusy = false;
        private ICommand _loadCommand;

        public CaptionAsset CaptionAsset { get; set; }

        public string AssetName
        {
            get => _assetName;
            set => Set(ref _assetName, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref value, _isBusy);
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm, CanConfirm); }
        }

        public Action CloseAction { get; set; }

        private bool CanConfirm()
        {
            return true;
        }

        private void Confirm()
        {
            MessageCenter.Instance.Send(new CloudMedia.CaptionAssetRenameRequestedMessage(this, CaptionAsset, AssetName));
            CloseAction?.Invoke();
        }

        private void Load()
        {
            AssetName = CaptionAsset?.Name;
        }
    }
}
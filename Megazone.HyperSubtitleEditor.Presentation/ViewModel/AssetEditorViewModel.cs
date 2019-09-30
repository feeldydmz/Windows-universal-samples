using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    public class AssetEditorViewModel : ViewModelBase
    {
        private string _assetName;

        private ICommand _confirmCommand;

        private bool _isBusy = false;
        private ICommand _loadCommand;

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

        private bool CanConfirm()
        {
            return true;
        }

        private void Confirm()
        {
        }

        private void Load()
        {
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class CaptionAssetListViewModel : ViewModelBase
    {
        private IEnumerable<CaptionAssetItemViewModel> _captionAssetItems = null;
        private CaptionAssetItemViewModel _selectedCaptionAssetItem;

        private ICommand _captionAssetSectionChangedCommand;
        private ICommand _captionElementSelectionChangedCommand;

        private bool _isNewCaptionAsset;

        
        public ICommand LeftButtonUpCommand
        {
            get
            {
                return _captionAssetSectionChangedCommand =
                    _captionAssetSectionChangedCommand ?? new RelayCommand<CaptionAssetItemViewModel>(CaptionAsstSelectionChanged);
            }
        }

        public ICommand CaptionElementSelectionChangedCommand
        {
            get
            {
                return _captionElementSelectionChangedCommand = _captionElementSelectionChangedCommand ??
                                                         new RelayCommand<CaptionElementItemViewModel>(
                                                             OnCaptionSelectionChanged);
            }
        }

        
        public IEnumerable<CaptionAssetItemViewModel> CaptionAssetItems
        {
            get => _captionAssetItems;
            set => Set(ref _captionAssetItems, value);
        }

        public bool IsCaptionAssetEmpty
        {
            get
            {
                // Id가 없는 CaptionAsset이 있다면 (캡션 새로 만들기 버튼임)
                if (_captionAssetItems != null && _captionAssetItems.Any(c => string.IsNullOrEmpty(c.Id)))
                {
                    return !(_captionAssetItems.Any(c => !string.IsNullOrEmpty(c.Id)));
                }

                return false;
            }
        }

        public CaptionAssetItemViewModel SelectedCaptionAssetItem
        {
            get => _selectedCaptionAssetItem;
            set => Set(ref _selectedCaptionAssetItem, value);
        }

        public bool IsNewCaptionAsset
        {
            get => _isNewCaptionAsset;
            set => Set(ref _isNewCaptionAsset, value);
        }

        public CaptionAssetListViewModel(IEnumerable<CaptionAssetItemViewModel> captionAssetItems = null)
        {
            CaptionAssetItems = captionAssetItems;

            if (CaptionAssetItems == null) return;
            
            SelectFirstItem();
        }

        private void SelectFirstItem()
        {
            var captionAssetItemViewModels = CaptionAssetItems.ToList();
            if (captionAssetItemViewModels.Any())
            {
                SelectedCaptionAssetItem = captionAssetItemViewModels.First();
                CaptionAsstSelectionChanged(SelectedCaptionAssetItem);
            }
        }

        private bool CanCaptionSelectionChanged(CaptionElementItemViewModel arg)
        {
            return SelectedCaptionAssetItem?.Elements?.Any(element => element.Equals(arg)) ?? false;
        }

        private void OnCaptionSelectionChanged(CaptionElementItemViewModel item)
        {
            var captionAssetItem = CaptionAssetItems.SingleOrDefault(assetItem =>
                assetItem.Elements?.Any(element => element.Equals(item)) ?? false);

            if (!SelectedCaptionAssetItem?.Equals(captionAssetItem) ?? true)
            {
                CaptionAssetItems?.ToList().ForEach(asset =>
                {
                    if (!asset.Equals(captionAssetItem))
                        asset.Initialize();
                });

                if (SelectedCaptionAssetItem != captionAssetItem)
                    SelectedCaptionAssetItem = captionAssetItem;
            }

            var isAnySeleted = captionAssetItem.Elements?.Any(element => element.IsSelected);

            if (isAnySeleted == false) SelectedCaptionAssetItem = null;
        }


        private void CaptionAsstSelectionChanged(CaptionAssetItemViewModel selectedItem)
        {
            SelectedCaptionAssetItem = SelectedCaptionAssetItem ?? selectedItem;

            SelectedCaptionAssetItem?.SelectAll();
            if (CaptionAssetItems != null)
                foreach (var captionAssetItem in CaptionAssetItems)
                    if (!captionAssetItem.Equals(SelectedCaptionAssetItem))
                        captionAssetItem.Initialize();

            IsNewCaptionAsset = SelectedCaptionAssetItem?.Source == null;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}

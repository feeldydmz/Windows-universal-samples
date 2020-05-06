using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Unity;

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

        
        public ICommand CaptionAssetSectionChangedCommand
        {
            get
            {
                return _captionAssetSectionChangedCommand =
                    _captionAssetSectionChangedCommand ?? new RelayCommand(OnCaptionAssetSectionChanged);
            }
        }

        public ICommand CaptionElementSelectionChangedCommand
        {
            get
            {
                return _captionElementSelectionChangedCommand = _captionElementSelectionChangedCommand ??
                                                         new RelayCommand<CaptionElementItemViewModel>(
                                                             OnCaptionSelectionChanged, CanCaptionSelectionChanged);
            }
        }

        public IEnumerable<CaptionAssetItemViewModel> CaptionAssetItems
        {
            get => _captionAssetItems;
            set => Set(ref _captionAssetItems, value);
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
        }

        private bool CanCaptionSelectionChanged(CaptionElementItemViewModel arg)
        {
            return SelectedCaptionAssetItem?.Elements?.Any(element => element.Equals(arg)) ?? false;
        }

        private void OnCaptionSelectionChanged(CaptionElementItemViewModel item)
        {
            if (SelectedCaptionAssetItem == null)
                return;

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


        private void OnCaptionAssetSectionChanged()
        {
            SelectedCaptionAssetItem?.SelectAll();
            if (CaptionAssetItems != null)
                foreach (var captionAssetItem in CaptionAssetItems)
                    if (!captionAssetItem.Equals(SelectedCaptionAssetItem))
                        captionAssetItem.Initialize();

            IsNewCaptionAsset = SelectedCaptionAssetItem?.Source == null;

            Debug.WriteLine($"IsNewCaptionAsset : {IsNewCaptionAsset}");

            CommandManager.InvalidateRequerySuggested();
        }
    }
}

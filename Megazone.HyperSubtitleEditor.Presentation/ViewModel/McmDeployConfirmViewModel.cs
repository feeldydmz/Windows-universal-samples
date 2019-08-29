using System;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal class McmDeployConfirmViewModel : ViewModelBase
    {
        private CaptionAssetItemViewModel _captionAssetItem;
        private string _url;

        private VideoItemViewModel _videoItem;
        
        public string Url
        {
            get => _url;
            set => Set(ref _url, value);
        }

        public VideoItemViewModel VideoItem
        {
            get => _videoItem;
            set => Set(ref _videoItem, value);
        }

        public CaptionAssetItemViewModel CaptionAssetItem
        {
            get => _captionAssetItem;
            set => Set(ref _captionAssetItem, value);
        }
    }
}
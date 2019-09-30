using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal class McmDeployConfirmViewModel : ViewModelBase
    {
        private CaptionAssetItemViewModel _captionAssetItem;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;

        private bool _hasLink;
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

        public IEnumerable<CaptionElementItemViewModel> CaptionItems
        {
            get => _captionItems;
            set => Set(ref _captionItems, value);
        }

        public bool HasLink
        {
            get => _hasLink;
            set => Set(ref _hasLink, value);
        }

        public void Update(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions, string linkUrl)
        {
            var captionItems = captions?.Select(caption => new CaptionElementItemViewModel(caption)).ToList();

            VideoItem = video != null ? new VideoItemViewModel(video) : null;
            CaptionAssetItem = captionAsset != null ? new CaptionAssetItemViewModel(captionAsset) : null;
            CaptionItems = captionItems;
            Url = linkUrl;

            HasLink = !string.IsNullOrEmpty(linkUrl);
        }
    }
}
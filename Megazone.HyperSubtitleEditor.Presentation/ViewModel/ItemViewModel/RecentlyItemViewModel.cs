using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class RecentlyItemViewModel : ViewModelBase
    {
        private IEnumerable<string> _captionElementIds;
        private bool _hasRelatedCaption;
        private string _id;
        private string _name;
        private RecentlyItem _recentlyItem;
        private string _relatedCaptionId;
        private string _relatedCaptionName;

        private Video _video;

        public RecentlyItemViewModel(RecentlyItem recentlyItem)
        {
            _recentlyItem = recentlyItem;

            _video = recentlyItem.Video;

            _name = recentlyItem.FirstName;
            _id = recentlyItem.FirstId;

            _hasRelatedCaption = _video != null && recentlyItem.CaptionAsset != null;
            _relatedCaptionName = recentlyItem.CaptionAsset?.Name;
            _relatedCaptionId = recentlyItem.CaptionAsset?.Id;

            _captionElementIds = recentlyItem.Captions?.Select(c => c.Id).ToList();
        }

        public Video Video
        {
            get => _video;
            set => Set(ref _video, value);
        }


        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string RelatedCaptionName
        {
            get => _relatedCaptionName;
            set => Set(ref _relatedCaptionName, value);
        }

        public string RelatedCaptionId
        {
            get => _relatedCaptionId;
            set => Set(ref _relatedCaptionId, value);
        }

        public bool HasRelatedCaption
        {
            get => _hasRelatedCaption;
            set => Set(ref _hasRelatedCaption, value);
        }

        public IEnumerable<string> CaptionElementIds
        {
            get => _captionElementIds;
            private set => Set(ref _captionElementIds, value);
        }
    }
}
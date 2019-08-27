using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class VideoItemViewModel : ViewModelBase
    {
        private IList<CaptionAssetItemViewModel> _captionItems;
        private string _primaryImageUrl;
        private CaptionAssetItemViewModel _selectedCaptionAsset;

        private int _totalCaptionCount;

        public VideoItemViewModel(Video video)
        {
            Id = video.Id;
            Name = video.Name;
            Description = video.Description;
            Status = video.Status;
            Duration = TimeSpan.FromMilliseconds(video.Duration);
            CreatedAt = string.IsNullOrEmpty(video.CreatedAt)
                ? DateTime.MinValue
                : DateTimeOffset.Parse(video.CreatedAt).DateTime;

            HasCaptions = video.Captions?.Any() ?? false;
            PrimaryImageUrl = GetPrimaryImage(video);
            Source = video;

            TotalCaptionCount = video.Captions?.Sum(asset => asset.Elements?.Count() ?? 0) ?? 0;
        }

        public CaptionAssetItemViewModel SelectedCaptionAsset
        {
            get => _selectedCaptionAsset;
            set => Set(ref _selectedCaptionAsset, value);
        }

        public IList<CaptionAssetItemViewModel> CaptionItems
        {
            get => _captionItems;
            set => Set(ref _captionItems, value);
        }


        public string PrimaryImageUrl
        {
            get => _primaryImageUrl;
            set => Set(ref _primaryImageUrl, value);
        }

        public int TotalCaptionCount
        {
            get => _totalCaptionCount;
            set => Set(ref _totalCaptionCount, value);
        }

        private int _selectedCaptionCount;
        public int SelectedCaptionCount
        {
            get => _selectedCaptionCount;
            set => Set(ref _selectedCaptionCount, value);
        }

        public bool HasCaptions { get; }
        public Video Source { get; private set; }
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Status { get; }
        public TimeSpan Duration { get; }
        public DateTime CreatedAt { get; }
        public bool CanUpdate { get; private set; } = true;

        private string GetPrimaryImage(Video video)
        {
            var url = string.Empty;
            if (video.Posters?.Any() ?? false)
                url = video.Posters.First().Url;

            if (video.Thumbnails?.Any() ?? false)
                url = video.Thumbnails.First().Elements?.FirstOrDefault()?.Url;

            return string.IsNullOrEmpty(url) ? null : url;
        }


        internal void UpdateSource(Video source)
        {
            Source = source;
            if (CaptionItems?.Any() ?? false)
                CaptionItems?.Clear();

            var list = source.Captions?.Select(asset => new CaptionAssetItemViewModel(asset)).ToList() ??
                       new List<CaptionAssetItemViewModel>();

            CaptionItems = new ObservableCollection<CaptionAssetItemViewModel>(list);
            TotalCaptionCount = list.Sum(asset => asset.Source.Elements?.Count() ?? 0);
            CanUpdate = false;
        }

        public void Update()
        {
            SelectedCaptionCount = this.SelectedCaptionAsset?.Elements.Count(element => element.IsSelected) ?? 0;
        }
    }
}
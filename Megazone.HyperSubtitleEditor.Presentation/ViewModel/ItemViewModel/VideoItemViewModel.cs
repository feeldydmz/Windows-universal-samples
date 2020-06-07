﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.View.LeftSideMenu;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class VideoItemViewModel : ViewModelBase
    {
        private IList<CaptionAssetItemViewModel> _captionAssetItems;
        private CaptionAssetListViewModel _captionAssetList;
        private IList<string> _encryptions;

        private bool _hasSelectedCaption;
        private string _primaryImageUrl;
        //private CaptionAssetItemViewModel _selectedCaptionAsset;

        private int _selectedCaptionCount;

        private int _totalCaptionCount;

        public VideoItemViewModel(Video video, bool isSearchResult = false)
        {
            Source = video;
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
            TotalCaptionCount = video.Captions?.Sum(asset => asset.Elements?.Count() ?? 0) ?? 0;

            if (isSearchResult)
            {
                CaptionAssetList = null;
            }
            else {
                var list = video.Captions?.Select(asset => new CaptionAssetItemViewModel(asset)).ToList() ??
                           new List<CaptionAssetItemViewModel>();

                CaptionAssetList = new CaptionAssetListViewModel(list);
            }

            Encryptions = video.Encryptions?.ToList();
            HasCaption = video.HasCaption;
        }

        public CaptionAssetListViewModel CaptionAssetList
        {
            get => _captionAssetList;
            set => Set(ref _captionAssetList, value);
        }

        public IList<string> Encryptions
        {
            get => _encryptions;
            set => Set(ref _encryptions, value);
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

        public int SelectedCaptionCount
        {
            get => _selectedCaptionCount;
            set => Set(ref _selectedCaptionCount, value);
        }

        public bool HasSelectedCaption
        {
            get => _hasSelectedCaption;
            set => Set(ref _hasSelectedCaption, value);
        }

        public bool HasCaptions { get; }
        public Video Source { get; private set; }
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Status { get; }
        public TimeSpan Duration { get; }
        public DateTime CreatedAt { get; }
        public bool HasCaption { get; }

        private string GetPrimaryImage(Video video)
        {
            var url = video.PrimaryPoster?.AccessUrl ?? video.ImageUrl;
            if (string.IsNullOrEmpty(url)) url = video.Posters?.FirstOrDefault(poster => poster.IsPreferred)?.AccessUrl;

            if (string.IsNullOrEmpty(url) && (video.Thumbnails?.Any() ?? false))
            {
                var thumbnail = video.Thumbnails.Count() > 1
                    ? video.Thumbnails.ToList()[1]
                    : video.Thumbnails.FirstOrDefault();

                url = thumbnail?.Urls?.FirstOrDefault();
                if (string.IsNullOrEmpty(url))
                {
                    var list = thumbnail?.Elements?.ToList() ?? new List<Thumbnail>();
                    url = list.Count() > 2 ? list[1]?.AccessUrl : list.FirstOrDefault()?.AccessUrl;
                }
            }

            return string.IsNullOrEmpty(url) ? null : url;
        }


        internal void UpdateSource(Video source)
        {
            Source = source;

            var list = source.Captions?.Select(asset => new CaptionAssetItemViewModel(asset)).ToList() ??
                       new List<CaptionAssetItemViewModel>();
          
            TotalCaptionCount = list.Sum(asset => asset.Source.Elements?.Count() ?? 0);

            list.Add(CaptionAssetItemViewModel.Empty);
            CaptionAssetList = new CaptionAssetListViewModel(list);
        }

        //public void Update()
        //{
        //    if (SelectedCaptionAsset?.Source != null)
        //    {
        //        SelectedCaptionCount = SelectedCaptionAsset?.Captions?.Count(element => element.IsSelected) ?? 0;
        //        HasSelectedCaption = SelectedCaptionCount > 0;
        //    }
        //    else
        //    {
        //        SelectedCaptionCount = 0;
        //        HasSelectedCaption = false;
        //    }

        //    if (!HasSelectedCaption &&
        //        SelectedCaptionAsset?.Captions != null &&
        //        SelectedCaptionAsset?.Captions.Count() != 0)
        //        SelectedCaptionAsset = null;
        //}

        //public void Initialize()
        //{
        //    SelectedCaptionAsset = null;
        //    SelectedCaptionCount = 0;
        //    HasSelectedCaption = false;
        //}
    }
}
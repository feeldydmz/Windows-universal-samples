﻿using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class VideoItemViewModel : ViewModelBase
    {
        private string _primaryImageUrl;

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
            //CaptionItems = video.Captions?.Select(asset => new CaptionAssetItemViewModel(asset)).ToList();
            Source = video;
        }

        private string GetPrimaryImage(Video video)
        {
            var url = string.Empty;
            if (video.Posters?.Any() ?? false)
                url = video.Posters.First().Url;

            if (video.Thumbnails?.Any() ?? false)
                url = video.Thumbnails.First().Elements?.FirstOrDefault()?.Url;

            return string.IsNullOrEmpty(url) ? null : url;
        }
        
        
        public string PrimaryImageUrl
        {
            get => _primaryImageUrl;
            set => Set(ref _primaryImageUrl, value);
        }

        public bool HasCaptions { get; }
        public Video Source { get; }
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Status { get; }
        public TimeSpan Duration { get; }
        public DateTime CreatedAt { get; }
    }
}
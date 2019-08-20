using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class AssetItemViewModel<TAssetElement> : ViewModelBase
        where TAssetElement : IAssetElement
    {
        private IEnumerable<TAssetElement> _elements;
        

        public AssetItemViewModel(Asset<TAssetElement> asset)
        {
            Id = asset.Id;
            Name = asset.Name;
            IsActive = asset.Status?.ToLower().Equals("active") ?? false;
            Type = asset.Type;
            MediaType = asset.MediaType;
            Duration = asset.Duration;
            Version = asset.Version;
            CreatedAt = string.IsNullOrEmpty(asset.CreatedAt)
                ? DateTime.MinValue
                : DateTimeOffset.Parse(asset.CreatedAt).DateTime;
            Elements = asset.Elements;
            Source = asset;
        }

        public Asset<TAssetElement> Source { get; }
        public string Id { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public DateTime CreatedAt { get; }

        public IEnumerable<TAssetElement> Elements
        {
            get => _elements;
            set => Set(ref _elements, value);
        }
    }
}
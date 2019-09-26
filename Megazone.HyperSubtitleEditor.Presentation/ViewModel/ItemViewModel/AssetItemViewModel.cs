using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class AssetItemViewModel<TAsset> : ViewModelBase where TAsset : IAsset
    {
        private IEnumerable _elements;

        public AssetItemViewModel(TAsset asset)
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
            Encryptions = asset.Encryptions;
            HasEncryptions = asset.Encryptions?.Any() ?? false;
            Source = asset;
        }

        public TAsset Source { get; }
        public string Id { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public IEnumerable<string> Encryptions { get; }
        public bool HasEncryptions { get; }
        public DateTime CreatedAt { get; }

        public IEnumerable Elements
        {
            get => _elements;
            set => Set(ref _elements, value);
        }
    }
}
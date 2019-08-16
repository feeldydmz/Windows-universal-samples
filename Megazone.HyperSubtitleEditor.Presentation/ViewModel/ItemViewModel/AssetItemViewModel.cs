using System;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class AssetItemViewModel : ViewModelBase
    {
        public AssetItemViewModel(Asset asset)
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
        }

        public string Id { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public DateTime CreatedAt { get; }
    }

    /// <summary>
    ///     Caption asset's element view model.
    /// </summary>
    public class CaptionElementItemViewModel : ViewModelBase
    {
        private bool _isSelected;
        public string Id { get; set; }
        public string Label { get; set; }
        public string Kind { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
    }
}
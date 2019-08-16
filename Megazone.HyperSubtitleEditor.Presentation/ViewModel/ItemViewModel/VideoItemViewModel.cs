using System;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class VideoItemViewModel : ViewModelBase
    {
        public VideoItemViewModel(Video video)
        {
            Id = video.Id;
            Name = video.Name;
            Description = video.Description;
            Status = video.Status;
            Duration = video.Duration;
            CreatedAt = string.IsNullOrEmpty(video.CreatedAt)
                ? DateTime.MinValue
                : DateTimeOffset.Parse(video.CreatedAt).DateTime;

            HasCaptions = video.Captions?.Any() ?? false;
        }

        public bool HasCaptions { get; }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Status { get; }
        public long Duration { get; }
        public DateTime CreatedAt { get; }
    }
}
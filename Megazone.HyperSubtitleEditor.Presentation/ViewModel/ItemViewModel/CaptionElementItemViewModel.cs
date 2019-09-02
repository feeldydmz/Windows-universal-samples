using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    /// <summary>
    ///     Caption asset's element view model.
    /// </summary>
    internal class CaptionElementItemViewModel : ViewModelBase
    {
        private bool _isDraft;
        private bool _isPreferred;
        private bool _isSelected;

        public CaptionElementItemViewModel(Caption caption)
        {
            Source = caption;
            Country = caption.Country;
            Id = caption.Id;
            IsDraft = caption.IsDraft;
            IsPreferred = caption.IsPreferred;
            Kind = caption.Kind;
            Label = caption.Label;
            Language = caption.Language;
            FileUrl = caption.Url;
        }

        public Caption Source { get; }

        public string Id { get; }
        public string Label { get; }
        public string Kind { get; }
        public string Country { get; }
        public string Language { get; }
        public string FileUrl { get; }

        public bool IsDraft
        {
            get => _isDraft;
            set => Set(ref _isDraft, value);
        }

        public bool IsPreferred
        {
            get => _isPreferred;
            set => Set(ref _isPreferred, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
    }
}
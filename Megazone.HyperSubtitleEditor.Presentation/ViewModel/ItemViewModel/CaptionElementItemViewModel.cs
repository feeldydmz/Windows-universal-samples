using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    /// <summary>
    ///     Caption asset's element view model.
    /// </summary>
    public class CaptionElementItemViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isDraft;
        private bool _isPreferred;

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
            Url = caption.Url;
        }
        public Caption Source { get; }

        public string Id { get; }
        public string Label { get; }
        public string Kind { get; }
        public string Country { get; }
        public string Language { get; }
        public string Url { get; }

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

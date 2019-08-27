using System;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;

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
            Text = (caption as WorkContext.CaptionContext)?.Text;
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

        public string Text { get; set; }

        public string GetFileName()
        {
            var caption = Source;
            var url = caption.Url;
            if (string.IsNullOrEmpty(url))
            {
                return $"{caption.Label}_{caption.Language}_{DateTime.UtcNow.DateTimeToEpoch()}.vtt";
            }

            var lastSlashIndex = url.LastIndexOf('/');

            var fileName = url.Substring(lastSlashIndex + 1, url.Length - lastSlashIndex - 1);
            return fileName;
        }
    }
}

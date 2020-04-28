using System;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    /// <summary>
    ///     Caption asset's element view model.
    /// </summary>
    internal class CaptionElementItemViewModel : ViewModelBase
    {
        private bool _canDeploy;
        private bool _isDraft;
        private bool _isPreferred;
        private bool _isSelected;
        private bool _isDirty;
        private bool _isOpened;

        public CaptionElementItemViewModel(Caption caption)
        {
            Source = caption;
            Country = caption.Country;
            Id = caption.Id;
            IsDraft = caption.IsDraft;
            IsPreferred = caption.IsPreferred;
            Kind = GetCaptionKind(caption.Kind);
            Label = caption.Label;
            Language = caption.Language;
            FileUrl = caption.Url;
        }

        private CaptionKind GetCaptionKind(string kind)
        {
            Enum.TryParse(kind, true, out CaptionKind captionKind);

            return captionKind;
        }

        public Caption Source { get; }

        public string Id { get; }
        public string Label { get; }
        public CaptionKind Kind { get; }
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

        public bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }


        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                Set(ref _isOpened, value);

                IsSelected = _isOpened;
            }
        }


        public bool CanDeploy
        {
            get => _canDeploy;
            set => Set(ref _canDeploy, value);
        }
    }
}
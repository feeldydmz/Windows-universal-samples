﻿using System;
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
        private bool _isDirty;
        private bool _isDraft;
        private bool _isOpened;
        private bool _isPreferred;
        private bool _isSelected;
        private SourceLocationKind _sourceLocation;

        public CaptionElementItemViewModel(Caption caption,
            SourceLocationKind sourceLocation = SourceLocationKind.CreatedByEditor)
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
            SourceLocation = sourceLocation;
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

        public SourceLocationKind SourceLocation
        {
            get => _sourceLocation;
            set => Set(ref _sourceLocation, value);
        }

        private CaptionKind GetCaptionKind(string kind)
        {
            Enum.TryParse(kind, true, out CaptionKind captionKind);

            return captionKind;
        }
    }
}
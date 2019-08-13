using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Megazone.Api.Transcoder.Domain;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.SubtitleEditor.Resources;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class JobItemViewModel : ViewModelBase, IMediaItem
    {
        private const int DEFAULT_BUFFERING_SECONDS = 5;
        private readonly IBrowser _browser;

        private ICommand _copyJobIdCommand;

        private bool _hasAudioOnly;
        private decimal _naturalDuration;
        private BitmapSource _thumbnailSource;
        private string _url;

        private int _videoBufferingSeconds = DEFAULT_BUFFERING_SECONDS;

        public JobItemViewModel(IBrowser browser, Job job)
        {
            _browser = browser;
            Data = job;
            Id = job.Payload.JobId;
        }

        public Job Data { get; }

        public string OutputKeyPrefix { get; internal set; }

        public string Id { get; }

        public string InputVideoName { get; set; }

        public DateTime FinishTime { get; set; }

        public string Status { get; set; }

        public decimal NaturalDuration
        {
            get => _naturalDuration;
            private set => Set(ref _naturalDuration, value);
        }

        public BitmapSource ThumbnailSource
        {
            get => _thumbnailSource;
            private set => Set(ref _thumbnailSource, value);
        }

        public IList<JobListItemOutputViewModel> Outputs { get; set; } = new List<JobListItemOutputViewModel>();

        public IList<JobListItemOutputViewModel> DisplayOutputs => Outputs?.ToList();

        public bool IsExistMoreDisplayOutput => Outputs.Count > 3;

        public int VideoBufferingSeconds
        {
            get => _videoBufferingSeconds;
            set => Set(ref _videoBufferingSeconds, value);
        }

        public bool HasAudioOnly
        {
            get => _hasAudioOnly;
            set => Set(ref _hasAudioOnly, value);
        }

        public ICommand CopyJobIdCommand
        {
            get { return _copyJobIdCommand = _copyJobIdCommand ?? new RelayCommand(CopyJobId); }
        }

        public string Url
        {
            get => _url;
            set => Set(ref _url, value);
        }

        public void SetMediaHeaderData(MediaHeaderData mediaData)
        {
            NaturalDuration = mediaData?.NaturalDuration ?? 0M;
            var hasVideo = mediaData?.HasVideo ?? false;
            if (!hasVideo)
                HasAudioOnly = mediaData?.HasAudio ?? false;
        }

        public void SetThumbnail(BitmapSource thumbnail)
        {
            ThumbnailSource = thumbnail;
        }

        private void CopyJobId()
        {
            Clipboard.SetText(Id);
            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                string.Format(Resource.MSG_JOB_ID_COPY_TO_CLIPBOARD, Id),
                MessageBoxButton.OK));
        }

        public bool ContainsKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return true;
            var isContainedInId = Id?.Contains(keyword) ?? false;
            var isContainedInInputSourceKey = InputVideoName?.Contains(keyword) ?? false;
            return isContainedInId || isContainedInInputSourceKey;
        }
    }
}
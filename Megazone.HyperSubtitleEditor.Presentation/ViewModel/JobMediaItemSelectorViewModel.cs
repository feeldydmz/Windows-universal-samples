using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.Core.Windows.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class JobMediaItemSelectorViewModel : ViewModelBase, IMediaItem
    {
        private const int DEFAULT_VIDEO_PLAYER_NETWORK_BUFFERING_SECONDS = 5;

        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _finishTime;

        private bool _hasAudioOnly;
        private string _jobId;

        private ICommand _loadedCommand;
        private int _loadingCount;
        private decimal _naturalDuration;

        private ICommand _okCommand;
        private bool _showLoading;
        private BitmapSource _thumbnailSource;
        private ICommand _unloadedCommand;
        private string _url;
        private int _videoBufferingSeconds = DEFAULT_VIDEO_PLAYER_NETWORK_BUFFERING_SECONDS;

        public JobMediaItemSelectorViewModel()
        {
            JobId = AppContext.Job.Id;
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public ICommand UnloadedCommand
        {
            get { return _unloadedCommand = _unloadedCommand ?? new RelayCommand(OnUnloaded); }
        }

        public IList<JobListItemOutputViewModel> OutputList { get; } =
            new ObservableCollection<JobListItemOutputViewModel>();

        public string JobId
        {
            get => _jobId;
            private set => Set(ref _jobId, value);
        }

        public DateTime FinishTime
        {
            get => _finishTime;
            set => Set(ref _finishTime, value);
        }

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

        public bool ShowLoading
        {
            get => _showLoading;
            set => Set(ref _showLoading, value);
        }

        public int LoadingCount
        {
            get => _loadingCount;
            set
            {
                Set(ref _loadingCount, value);
                ShowLoading = _loadingCount > 0;
            }
        }

        public ICommand OkCommand
        {
            get { return _okCommand = _okCommand ?? new RelayCommand(OnOk, CanOk); }
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

        private bool CanOk()
        {
            return OutputList != null && OutputList.Any(o => o.IsChecked);
        }

        private void OnOk()
        {
            var selectedMediaItem = OutputList.First(o => o.IsChecked);
            MessageCenter.Instance.Send(new MediaPlayer.OpenMediaFromUrlMessage(this, selectedMediaItem.FullUrl));
        }

        private void OnLoaded()
        {
            LoadJob();
        }

        private void OnUnloaded()
        {
        }

        private async void LoadJob()
        {
            LoadingCount++;
            await this.CreateTask(async () =>
            {
                FinishTime = AppContext.Job.Timing?.FinishTimeMillis?.EpocToDateTime() ?? DateTime.MinValue;

                var ouputItems = await new JobListItemOutputViewModelBuilder().BuildAsync(AppContext.Job);
                this.InvokeOnUi(async () =>
                {
                    OutputList.Clear();
                    foreach (var output in ouputItems)
                        OutputList.Add(output);

                    await this.CreateTask(() =>
                    {
                        var firstOutput = OutputList.FirstOrDefault();
                        if (firstOutput != null)
                        {
                            Url = firstOutput.FullUrl;
                            if (!_cancellationTokenSource?.Token.IsCancellationRequested ?? false)
                                _cancellationTokenSource?.Cancel();

                            _cancellationTokenSource = new CancellationTokenSource();

                            new MediaDataLoader(new List<IMediaItem>
                                {
                                    this
                                },
                                _cancellationTokenSource).Run();
                        }
                    });
                });
            });
            LoadingCount--;
        }
    }
}
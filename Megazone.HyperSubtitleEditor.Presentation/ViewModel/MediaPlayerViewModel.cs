using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Megazone.Core.Extension;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.VideoTrack;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.Core.Windows.Control.VideoPlayer.MediaControllers.FFmpegControls;
using Megazone.Core.Windows.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal class MediaPlayerViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly Action<MediaPlayStates> _onMediaPlayStateChanged;
        private readonly Action<decimal> _onMediaPositionChanged;
        private readonly SignInViewModel _signinViewModel;
        private CancellationTokenSource _cancellationTokenSource;
        private ICommand _changeToOriginalSourceCommand;

        private IList<IText> _currentPositionText;
        private VideoResolutionInfo _currentResolution;
        private MediaKind _currentVideoType;
        private ICommand _dropToSetMediaCommand;
        private bool _hasAudioOnly;
        private MediaHeaderData _headerData;

        private bool _isPreview;
        private string _mediaSource;
        private decimal _naturalDuration;

        private ICommand _playStateChangedCommand;
        private ICommand _positionChangedCommand;
        private IEnumerable<VideoResolutionInfo> _resolutions;

        private int _seekCount;
        private int _streamIndex;
        private BitmapSource _thumbnailSource;
        private MediaTimeSeeker _timeSeeker = new MediaTimeSeeker();

        private IEnumerable<MediaKind> _videoTypes;

        public MediaPlayerViewModel(Action<decimal> onMediaPositionChanged,
            Action<MediaPlayStates> onMediaPlayStateChanged)
        {
            _onMediaPositionChanged = onMediaPositionChanged;
            _onMediaPlayStateChanged = onMediaPlayStateChanged;
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _signinViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
        }

        public ICommand DropToSetMediaCommand
        {
            get
            {
                return
                    _dropToSetMediaCommand =
                        _dropToSetMediaCommand ?? new RelayCommand<object>(DropToSetMedia, CanDropToSetMedia);
            }
        }

        public BitmapSource ThumbnailSource
        {
            get => _thumbnailSource;
            set => Set(ref _thumbnailSource, value);
        }

        public decimal NaturalDuration
        {
            get => _naturalDuration;
            set => Set(ref _naturalDuration, value);
        }

        public bool HasAudioOnly
        {
            get => _hasAudioOnly;
            set => Set(ref _hasAudioOnly, value);
        }

        public string MediaSource
        {
            get => _mediaSource;
            set => Set(ref _mediaSource, value);
        }

        public int StreamIndex
        {
            get => _streamIndex;
            set => Set(ref _streamIndex, value);
        }

        public MediaTimeSeeker TimeSeeker
        {
            get => _timeSeeker;
            set => Set(ref _timeSeeker, value);
        }

        public MediaHeaderData HeaderData
        {
            get => _headerData;
            set => Set(ref _headerData, value);
        }

        public IList<IText> CurrentPositionText
        {
            get => _currentPositionText;
            set => Set(ref _currentPositionText, value);
        }

        public string ProjectId => _signinViewModel?.SelectedProject?.ProjectId;

        public ICommand PositionChangedCommand
        {
            get
            {
                return _positionChangedCommand =
                    _positionChangedCommand ?? new RelayCommand<decimal>(OnPositionChanged);
            }
        }

        public ICommand ChangeToOriginalSourceCommand
        {
            get
            {
                return _changeToOriginalSourceCommand =
                    _changeToOriginalSourceCommand ?? new RelayCommand(OnChangeToOriginalSource);
            }
        }

        /// <summary>
        ///     Binding one way
        /// </summary>
        public bool IsMediaPlaying { get; set; }

        public ICommand PlayStateChangedCommand
        {
            get
            {
                return
                    _playStateChangedCommand =
                        _playStateChangedCommand ?? new RelayCommand<MediaPlayStates>(OnMediaPlayStateChanged);
            }
        }

        public MediaKind CurrentVideoType
        {
            get => _currentVideoType;
            set
            {
                Set(ref _currentVideoType, value);

                if (_currentVideoType == null) return;

                VideoUrlOfResolutions = VideoResolutionsByType[_currentVideoType];
                Resolutions = VideoUrlOfResolutions.Keys;

                if (!Resolutions.IsNullOrEmpty())
                    CurrentResolution = Resolutions.First();
            }
        }

        public VideoResolutionInfo CurrentResolution
        {
            get => _currentResolution;
            set
            {
                Set(ref _currentResolution, value);

                if (_currentResolution == null) return;

                var url = VideoUrlOfResolutions[_currentResolution];

                Debug.WriteLine($"media url : {url}");

                FindStreamIndex();

                OpenMedia(url, IsLocalFile, _currentResolution);
            }
        }

        public IEnumerable<MediaKind> VideoTypes
        {
            get => _videoTypes;
            set => Set(ref _videoTypes, value);
        }

        public IEnumerable<VideoResolutionInfo> Resolutions
        {
            get => _resolutions;
            set => Set(ref _resolutions, value);
        }

        public Dictionary<VideoResolutionInfo, string> VideoUrlOfResolutions { get; private set; }

        public Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>> VideoResolutionsByType
        {
            get;
            private set;
        }

        public bool IsPreview
        {
            get => _isPreview;
            set => Set(ref _isPreview, value);
        }

        private McmWorkContext WorkContext { get; set; }
        private McmWorkContext OriginWorkContext { get; set; }

        /// <summary>
        ///     Binding Mode: One way to source
        /// </summary>
        public decimal CurrentMediaPosition { get; set; }

        // TODO : Video 를 받아 처리하도록 개선 필요.
        public bool IsLocalFile { get; set; }

        public MediaPlayStates PlayState { get; set; }

        private void OnMediaPlayStateChanged(MediaPlayStates state)
        {
            Debug.WriteLine($"OnMediaPlayStateChanged : {state}");
            PlayState = state;

            switch (state)
            {
                case MediaPlayStates.Opened:
                    OnOpenedVideo();
                    break;
                case MediaPlayStates.Buffering:
                    break;
                case MediaPlayStates.Closed:
                case MediaPlayStates.Manual:
                case MediaPlayStates.Seeking:
                case MediaPlayStates.Play:
                case MediaPlayStates.Pause:
                case MediaPlayStates.Stop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }


            _onMediaPlayStateChanged?.Invoke(state);
        }

        private void OnPositionChanged(decimal value)
        {
            _onMediaPositionChanged?.Invoke(value);
        }

        private void OnChangeToOriginalSource()
        {
            var videoMediaUrl = OriginWorkContext?.VideoMediaUrl;

            InitMedia(OriginWorkContext, true);
        }

        private void DropToSetMedia(object parameter)
        {
            if (!(parameter is IDataObject dataObject)) return;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return;
                var firstFilePath = filePaths.First();
                if (File.Exists(firstFilePath))
                    InitMedia(firstFilePath, true);
            }
        }

        public void InitMedia(McmWorkContext mcmWorkContext, bool isVideoContainer)
        {
            WorkContext = mcmWorkContext;
            IsLocalFile = false;

            IsPreview = !isVideoContainer;
            if (isVideoContainer) OriginWorkContext = WorkContext;

            VideoResolutionsByType = WorkContext.VideoResolutionsByType;
            VideoUrlOfResolutions = WorkContext.VideoUrlOfResolutions;

            VideoTypes = VideoResolutionsByType.Keys;
            //Resolutions = VideoResolutionsByType.First().Value?.Keys.OrderByDescending(resolution=>resolution);

            CurrentVideoType = VideoTypes.First();

            MediaSource = mcmWorkContext.VideoMediaUrl;
        }

        public void InitMedia(string filePath, bool isLocal)
        {
            IsLocalFile = isLocal;
            IsPreview = true;

            var videoHeaderData = GetVideoHeaderData(filePath);

            if (videoHeaderData != null && videoHeaderData.MpegDashStreamIndex != null)
            {
                VideoResolutionsByType = new Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>>();
                var resolutionItem = new Dictionary<VideoResolutionInfo, string>();

                var codec = videoHeaderData.VideoCodec.Split('_').Last();

                foreach (var item in videoHeaderData.MpegDashStreamIndex)
                    resolutionItem.Add(new VideoResolutionInfo(item.Value.Width, item.Value.Height, codec), filePath);

                VideoResolutionsByType.Add(new MediaKind(videoHeaderData.VideoKindName.ToUpper(), filePath),
                    resolutionItem);

                VideoTypes = VideoResolutionsByType.Keys;

                CurrentVideoType = VideoTypes.First();
            }
        }

        public async void OpenMedia(string firstFilePath, bool isLocalFile, VideoResolutionInfo resolution = null)
        {
            try
            {
                GetThumbnailAsync(new Uri(firstFilePath), isLocalFile);

                MediaSource = "";

                this.CreateTask(() => { MediaSource = firstFilePath; });
            }
            catch (Exception ex)
            {
                _logger.Error.Write("OpenMedia : ", ex.Message);
                throw ex;
            }
        }

        private void FindStreamIndex()
        {
            if (HeaderData?.MpegDashStreamIndex != null && CurrentResolution != null)
                try
                {
                    var indexValue =
                        HeaderData.MpegDashStreamIndex.Single(item =>
                            item.Value.Height.Equals(CurrentResolution.Height));
                    StreamIndex = indexValue.Key;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.Error.Write(ex.Message);
                }
        }

        private void OnOpenedVideo()
        {
            Debug.WriteLine("+ OnOpendVideo");

            SetMediaHeaderData(null);

            //if (!IsLocalFile)
            //    //MPEG-DASH 재생 해상도의 인덱스를 구한다
            //    if (HeaderData?.MpegDashStreamIndex != null && CurrentResolution != null)
            //        try
            //        {
            //            var indexValue =
            //                HeaderData.MpegDashStreamIndex.Single(item =>
            //                    item.Value.Height.Equals(CurrentResolution.Height));
            //            StreamIndex = indexValue.Key;
            //        }
            //        catch (InvalidOperationException ex)
            //        {
            //            _logger.Error.Write(ex.Message);
            //        }

            SetMediaHeaderData(HeaderData);

            Debug.WriteLine("OnOpendVideo 3");

            if (HeaderData != null)
                //var fullPath = IsLocalFile ? HeaderData.Source.LocalPath : HeaderData.Source.AbsoluteUri;
                LoadMediaItem(HeaderData.Source, IsLocalFile, HeaderData);

            Debug.WriteLine("OnOpendVideo 4");
        }

        private string GetTempThumbnailFilePath(Uri uri, bool isLocalFile)
        {
            var folderPath = this.TempFolder();
            if (folderPath == null)
                return null;

            var fileHeader = Guid.NewGuid().ToString() + DateTime.UtcNow.DateTimeToEpoch();
            if (isLocalFile)
            {
                var fi = new FileInfo(uri.LocalPath);
                if (!fi.Exists) return null;
                return folderPath + "\\" + fileHeader + ".jpg";
            }

            return folderPath + "\\" + fileHeader + ".jpg";
        }

        internal void RemoveMediaItem()
        {
            MediaSource = null;
            ThumbnailSource = null;
            SetMediaHeaderData(null);
        }

        private MediaHeaderData GetVideoHeaderData(string fullPath)
        {
            var authorization = getAuthorization();
            var projectId = _signinViewModel.SelectedProject?.ProjectId;

            var videoData = VideoHeaderHelper.GetVideoHeaderData(new GetVideoDataParameters
            {
                Url = fullPath,
                IsRequestThumbnail = false,
                Headers = $"mz-cm-auth:Bearer {authorization}\r\nprojectId:{projectId}"
            }, _cancellationTokenSource);

            return videoData;
        }


        private async void LoadMediaItem(Uri uri, bool isLocalFile, MediaHeaderData videoData)
        {
            ThumbnailSource = null;
            //SetMediaHeaderData(null);
            _cancellationTokenSource = new CancellationTokenSource();
            var authorization = getAuthorization();

            var isAdaptiveHttpStreaming =
                HeaderData.MediaKind == MediaKinds.Dash || HeaderData.MediaKind == MediaKinds.Hls;

            //this.CreateTask(() =>
            //{
            //    try
            //    {
            //        //var videoData = VideoHeaderHelper.GetVideoHeaderData(new GetVideoDataParameters
            //        //{
            //        //    Url = fullPath,
            //        //    IsRequestThumbnail = false
            //        //}, _cancellationTokenSource);
            //        if (videoData != null)
            //        {
            //            var thumbnail = new FFmpegLauncher().GetThumbnail(new FFmpegLauncher.FFmpegLauncherParameter
            //            {
            //                SaveThumbnailPath = GetTempThumbnailFilePath(uri, isLocalFile),
            //                TimeoutMilliseconds = 7000,
            //                LocalFilePath = isLocalFile ? uri.LocalPath : uri.AbsoluteUri,
            //                ThumbnailAtSeconds = (double) videoData.StartTime,
            //                Headers = isAdaptiveHttpStreaming ? $"mz-cm-auth:Bearer {authorization}" : null
            //            });
            //            if (thumbnail != null)
            //                this.InvokeOnUi(() => { ThumbnailSource = thumbnail; });
            //            //this.InvokeOnUi(() =>
            //            //{
            //            //    _videoData = videoData;
            //            //    SetMediaHeaderData(_videoData);
            //            //});
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.Error.Write(ex);
            //    }
            //});
        }

        private async void GetThumbnailAsync(Uri uri, bool isLocalFile)
        {
            ThumbnailSource = null;
            //SetMediaHeaderData(null);
            _cancellationTokenSource = new CancellationTokenSource();
            var authorization = getAuthorization();
            var projectId = _signinViewModel.SelectedProject?.ProjectId;

            var workBarViewmodel = Bootstrapper.Container.Resolve<WorkBarViewModel>();


            //var isAdaptiveHttpStreaming = HeaderData.MediaKind == MediaKinds.Dash || HeaderData.MediaKind == MediaKinds.Hls;
            var isAdaptiveHttpStreaming = false;

            var extension = Path.GetExtension(uri.LocalPath).Replace(".", "");

            if (extension.Equals("mpd") || extension.Equals("m3u8")) isAdaptiveHttpStreaming = true;

            this.CreateTask(() =>
            {
                try
                {
                    var thumbnail = new FFmpegLauncher().GetThumbnail(new FFmpegLauncher.FFmpegLauncherParameter
                    {
                        SaveThumbnailPath = GetTempThumbnailFilePath(uri, isLocalFile),
                        TimeoutMilliseconds = 7000,
                        LocalFilePath = isLocalFile ? uri.LocalPath : uri.AbsoluteUri,
                        ThumbnailAtSeconds = 2,
                        Headers = isAdaptiveHttpStreaming
                            ? $"mz-cm-auth:Bearer {authorization}\r\nprojectId:{projectId}"
                            : null
                    });
                    if (thumbnail != null)
                        this.InvokeOnUi(() => { ThumbnailSource = thumbnail; });
                }
                catch (Exception ex)
                {
                    _logger.Error.Write(ex);
                }
            });
        }

        private void SetMediaHeaderData(MediaHeaderData data)
        {
            NaturalDuration = data?.NaturalDuration ?? 0M;
            var hasVideo = data?.HasVideo ?? false;
            if (!hasVideo)
                HasAudioOnly = data?.HasAudio ?? false;
        }

        private bool CanDropToSetMedia(object parameter)
        {
            if (!(parameter is IDataObject dataObject))
                return true;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return false;
                // 라이브러리 or 하드 드라이브는 드롭할 수 없다.
                var logicalDrives = Environment.GetLogicalDrives();
                var hasDrive = filePaths.Any(f => logicalDrives.Contains(f.ToUpper()));
                if (hasDrive)
                    return false;
                if (filePaths.Count > 1) return false;
                return !filePaths.Any(f =>
                {
                    if (Directory.Exists(f))
                        return true;
                    var fileExtension = Path.GetExtension(f);
                    return !(FileTypeDistributer.IsVideoFormat(fileExtension) ||
                             FileTypeDistributer.IsAudioFormat(fileExtension));
                });
            }

            return false;
        }

        public string getAuthorization()
        {
            return _signinViewModel.GetAuthorizationAsync().Result?.AccessToken;
            //return "mz-cm-auth:Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZTRjY2QzZi1jYWQ5LTQ0OWEtYjlhMC1kZmMzNzcxMDE4N2MiLCJleHAiOjE1ODE1MjQ3MjUsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJqdGkiOiIzNmIyZDcyZS1kMWZhLTQ4ZDUtYjhlNy0zNDdlNzIzNDNiOWIiLCJjbGllbnRfaWQiOiIwYTMxZTdkYy02NWViLTQ0MzAtOTAyNS0yNGY5ZTNkN2Q2MGQiLCJzY29wZSI6WyJvcmdhbml6YXRpb24udGVhbSIsIm9yZ2FuaXphdGlvbiIsInVzZXIuY29tcGFueSIsImRlZmF1bHQucHJvZmlsZSJdLCJhY3RpdmUiOnRydWV9.gGzK5Zz8t4p1QbhqXFEpjxaNXrxyAhZDdqwgIJLYCUUcBFJcsR8WS2APIlYgnFsBRkmm14DSufiwO3zG9Bn-dIisy-KKQr2shl6eKy92ExoBIPQQxFz8_qikmBm7N1EcfeiMVjKRoG6HXQ6TaN3rd2Myt-FtqJXbznVeVfEbNC8FvZPxuFwPC4z72hL_X4d4htoqj6-cpjLa_Ka0U2AzF10pgb4-ZRa6MscxuSTxEEZbwujjuutbo13wzFtoATU5eR3H9BlPFDRfBtIk5e1N5yqdnWhnf-iecyvtUDQbKIF1UfQ4O1bsFcACNjg7ONvRQndN1hNSJHnqXJkkRrnpnA";
        }

        public void SyncPosition(TimeSpan objStartTime)
        {
            var url = VideoUrlOfResolutions[_currentResolution];

            TimeSeeker?.Run(objStartTime, () => { _seekCount = 0; });
            //LoadMediaItem();

            //this.CreateTask(() =>
            //{
            //    try
            //    {
            //        //var videoData = VideoHeaderHelper.GetVideoHeaderData(new GetVideoDataParameters
            //        //{
            //        //    Url = fullPath,
            //        //    IsRequestThumbnail = false
            //        //}, _cancellationTokenSource);
            //        if (objStartTime != null)
            //        {
            //            var thumbnail = new FFmpegLauncher().GetThumbnail(new FFmpegLauncher.FFmpegLauncherParameter
            //            {
            //                SaveThumbnailPath = GetTempThumbnailFilePath(url, IsLocalFile),
            //                TimeoutMilliseconds = 7000,
            //                LocalFilePath = IsLocalFile ? url : new Uri(url).AbsoluteUri,
            //                ThumbnailAtSeconds = objStartTime.TotalSeconds
            //            });
            //            if (thumbnail != null)
            //                this.InvokeOnUi(() => { ThumbnailSource = thumbnail; });
            //            //this.InvokeOnUi(() =>
            //            //{
            //            //    _videoData = videoData;
            //            //    SetMediaHeaderData(_videoData);
            //            //});
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.Error.Write(ex);
            //    }
            //});
        }

        public void PlayForwardBy(TimeSpan timespan)
        {
            _seekCount++;
            var currentPosition = CurrentMediaPosition;
            var targetPosition = Convert.ToDouble(currentPosition) + timespan.TotalSeconds * _seekCount;
            var naturalDuration = Convert.ToDouble(NaturalDuration);
            if (targetPosition > naturalDuration)
                targetPosition = naturalDuration;

            SyncPosition(TimeSpan.FromSeconds(targetPosition));
        }

        public void PlayBackBy(TimeSpan timespan)
        {
            _seekCount--;
            var currentPosition = CurrentMediaPosition;
            var targetPosition = Convert.ToDouble(currentPosition) + timespan.TotalSeconds * _seekCount;
            if (targetPosition < 0)
                targetPosition = 0;

            SyncPosition(TimeSpan.FromSeconds(targetPosition));
        }
    }
}
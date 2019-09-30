using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.Extension;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.VideoTrack;
using Megazone.Core.Windows.Control.VideoPlayer;
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
        private CancellationTokenSource _cancellationTokenSource;

        private IList<IText> _currentPositionText;
        private ICommand _dropToSetMediaCommand;
        private bool _hasAudioOnly;
        private string _mediaSource;
        private decimal _naturalDuration;

        private ICommand _playStateChangedCommand;

        private ICommand _positionChangedCommand;

        private IEnumerable<MediaKind> _videoTypes;
        private IEnumerable<VideoResolutionInfo> _resolutions;
        private VideoResolutionInfo _currentResolution;
        private MediaKind _currentVideoType;

        private int _seekCount;
        private BitmapSource _thumbnailSource;
        private MediaTimeSeeker _timeSeeker = new MediaTimeSeeker();
        private MediaHeaderData _videoData;
        private int _streamIndex;

        public MediaPlayerViewModel(Action<decimal> onMediaPositionChanged,
            Action<MediaPlayStates> onMediaPlayStateChanged)
        {
            _onMediaPositionChanged = onMediaPositionChanged;
            _onMediaPlayStateChanged = onMediaPlayStateChanged;
            _logger = Bootstrapper.Container.Resolve<ILogger>();
        }

        public ICommand DropToSetMediaCommand
        {
            get
            {
                return
                    _dropToSetMediaCommand =
                        _dropToSetMediaCommand ?? new RelayCommand<object>(SetMediaCommand, CanDropToSetMedia);
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

        public IList<IText> CurrentPositionText
        {
            get => _currentPositionText;
            set => Set(ref _currentPositionText, value);
        }

        public ICommand PositionChangedCommand
        {
            get
            {
                return _positionChangedCommand =
                    _positionChangedCommand ?? new RelayCommand<decimal>(OnPositionChanged);
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

                if(!Resolutions.IsNullOrEmpty())
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

                OpenMedia(url, false, _currentResolution);
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

        public Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>> VideoResolutionsByType { get; private set; }

        private McmWorkContext WorkContext { get; set; }

        /// <summary>
        ///     Binding Mode: One way to source
        /// </summary>
        public decimal CurrentMediaPosition { get; set; }

        private void OnMediaPlayStateChanged(MediaPlayStates state)
        {
            _onMediaPlayStateChanged?.Invoke(state);
        }

        private void OnPositionChanged(decimal value)
        {
            _onMediaPositionChanged?.Invoke(value);
        }

        private void SetMediaCommand(object parameter)
        {
            if (!(parameter is IDataObject dataObject)) return;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return;
                var firstFilePath = filePaths.First();
                if (File.Exists(firstFilePath))
                    OpenMedia(firstFilePath, true);
            }
        }

        public void InitMedia(McmWorkContext mcmWorkContext, bool isLocalFile)
        {
            WorkContext = mcmWorkContext;

            VideoResolutionsByType = WorkContext.VideoResolutionsByType;
            VideoUrlOfResolutions = WorkContext.VideoUrlOfResolutions;

            VideoTypes = VideoResolutionsByType.Keys;
            //Resolutions = VideoResolutionsByType.First().Value?.Keys.OrderByDescending(resolution=>resolution);

            CurrentVideoType = VideoTypes.First();

            MediaSource = mcmWorkContext.VideoMediaUrl;
        }

        // url을 입력해서 동영상을 열때 사용
        //public void InitMedia(string urlPath, bool isLocalFile)
        //{
        //    MediaSource = urlPath;

        //    if (!MediaSource.IsNullOrEmpty())
        //    {
        //        var dashHeaderData = GetVideoHeaderData(MediaSource);

        //        if (dashHeaderData == null) return;

        //        VideoResolutionsByType = new Dictionary<string, Dictionary<int, string>>();
        //        VideoUrlOfResolutions = new Dictionary<int, string>();

        //        if (dashHeaderData.MpegDashStreamIndex != null)
        //        {
        //            foreach (var item in dashHeaderData?.MpegDashStreamIndex)
        //            {
        //                var index = item.Key;
        //                var height = item.Value;
        //                if (!VideoUrlOfResolutions.ContainsKey(height))
        //                    VideoUrlOfResolutions.Add(height, MediaSource);
        //            }

        //            VideoResolutionsByType.Add("DASH", VideoUrlOfResolutions);

        //        }
        //        else
        //        {

        //            var height = (int)(dashHeaderData.VideoFrameSize.Height);

        //            if (!VideoUrlOfResolutions.ContainsKey(height))
        //                VideoUrlOfResolutions.Add(height, MediaSource);

        //            VideoResolutionsByType.Add(dashHeaderData.VideoKindName, VideoUrlOfResolutions);
        //        }
        //    }

        //    VideoTypes = VideoResolutionsByType.Keys;
        //    Resolutions = VideoResolutionsByType.First().Value?.Keys;
        //}

        public void InitMedia(string filePath)
        {
            var videoHeaderData = GetVideoHeaderData(filePath);

            if (videoHeaderData != null && videoHeaderData.MpegDashStreamIndex != null)
            {
                VideoResolutionsByType = new Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>>();
                var resolutionItem = new Dictionary<VideoResolutionInfo, string>();

                var codec = videoHeaderData.VideoCodec.Split('_').Last();

                foreach (var item in videoHeaderData.MpegDashStreamIndex)
                {
                    resolutionItem.Add(new VideoResolutionInfo(item.Value.Width, item.Value.Height, codec), filePath);
                }

                VideoResolutionsByType.Add(new MediaKind(videoHeaderData.VideoKindName.ToUpper(), filePath), resolutionItem);

                VideoTypes = VideoResolutionsByType.Keys;
                //Resolutions = VideoResolutionsByType.First().Value?.Keys.OrderByDescending(resolution=>resolution);

                CurrentVideoType = VideoTypes.First();

                //MediaSource = filePath;
            }
        }

        public void OpenMedia(String firstFilePath, bool isLocalFile, VideoResolutionInfo resolution = null)
        {
            MediaSource = firstFilePath;
            
            var videoHeaderData = GetVideoHeaderData(firstFilePath);

            if (!isLocalFile)
            {
                //MPEG-DASH 재생 해상도의 인덱스를 구한다
                if (videoHeaderData?.MpegDashStreamIndex != null && resolution != null)
                {
                    try
                    {
                        var indexValue =
                            videoHeaderData.MpegDashStreamIndex.Single(item => item.Value.Height.Equals(resolution.Height));
                        StreamIndex = indexValue.Key;
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.Error.Write(ex.Message);
                    }
                }
            }

            LoadMediaItem(firstFilePath, isLocalFile, videoHeaderData);
        }

        private string GetTempThumbnailFilePath(string fullPath, bool isLocalFile = false)
        {
            var folderPath = this.TempFolder();
            if (folderPath == null)
                return null;

            var fileHeader = Guid.NewGuid().ToString() + DateTime.UtcNow.DateTimeToEpoch();
            if (isLocalFile)
            {
                var fi = new FileInfo(fullPath);
                if (!fi.Exists) return null;
                return folderPath + "\\" + fileHeader + "_" + fi.Name.Replace(fi.Extension, string.Empty) + ".jpg";
            }

            var lastIndexOfSlash = fullPath.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndexOfSlash == -1) return null;
            var fileName = fullPath.Substring(lastIndexOfSlash + 1);
            var indexOfFirstDot = fileName.IndexOf(".", StringComparison.Ordinal);
            if (indexOfFirstDot == -1) return null;
            fileName = fileName.Substring(0, indexOfFirstDot) + ".jpg";
            return folderPath + "\\" + fileHeader + "_" + fileName;
        }

        internal void RemoveMediaItem()
        {
            MediaSource = null;
            ThumbnailSource = null;
            SetMediaHeaderData(null);
        }

        private MediaHeaderData GetVideoHeaderData(string fullPath)
        {
            var videoData = VideoHeaderHelper.GetVideoHeaderData(new GetVideoDataParameters
            {
                Url = fullPath,
                IsRequestThumbnail = false
            }, _cancellationTokenSource);

            return videoData;
        }


        private void LoadMediaItem(string fullPath, bool isLocalFile, MediaHeaderData videoData)
        {
            ThumbnailSource = null;
            SetMediaHeaderData(null);
            _cancellationTokenSource = new CancellationTokenSource();
            this.CreateTask(() =>
            {
                try
                {
                    //var videoData = VideoHeaderHelper.GetVideoHeaderData(new GetVideoDataParameters
                    //{
                    //    Url = fullPath,
                    //    IsRequestThumbnail = false
                    //}, _cancellationTokenSource);
                    if (videoData != null)
                    {
                        var thumbnail = new FFmpegLauncher().GetThumbnail(new FFmpegLauncher.FFmpegLauncherParameter
                        {
                            SaveThumbnailPath = GetTempThumbnailFilePath(fullPath, isLocalFile),
                            TimeoutMilliseconds = 7000,
                            LocalFilePath = isLocalFile ? fullPath : new Uri(fullPath).AbsoluteUri,
                            ThumbnailAtSeconds = (double) videoData.StartTime
                        });
                        if (thumbnail != null)
                            this.InvokeOnUi(() => { ThumbnailSource = thumbnail; });
                        this.InvokeOnUi(() =>
                        {
                            _videoData = videoData;
                            SetMediaHeaderData(_videoData);
                        });
                    }
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

        public void SyncPosition(TimeSpan objStartTime)
        {
            TimeSeeker?.Run(objStartTime, () => { _seekCount = 0; });
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
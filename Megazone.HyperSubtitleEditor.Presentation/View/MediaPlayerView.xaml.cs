﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Megazone.Core.VideoTrack;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     Interaction logic for MediaPlayerView.xaml
    /// </summary>
    public partial class MediaPlayerView : UserControl
    {
        public static readonly DependencyProperty SubtitleFlowDocumentTextProperty =
            DependencyProperty.Register(
                "SubtitleFlowDocumentText",
                typeof(IList<IText>),
                typeof(MediaPlayerView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty NaturalDurationProperty =
            DependencyProperty.Register("NaturalDuration", typeof(decimal), typeof(MediaPlayerView),
                new PropertyMetadata(0M));

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register("CurrentPosition", typeof(decimal), typeof(MediaPlayerView),
                new PropertyMetadata(0M));

        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register("ThumbnailSource", typeof(BitmapSource), typeof(MediaPlayerView));

        public static readonly DependencyProperty MediaSourceProperty =
            DependencyProperty.Register("MediaSource", typeof(string), typeof(MediaPlayerView),
                new PropertyMetadata(
                    (s, e) => { ((MediaPlayerView) s).OnMediaSourceProperty((string)e.NewValue, (string) e.OldValue); }));

        public static readonly DependencyProperty StreamIndexProperty =
            DependencyProperty.Register("StreamIndex", typeof(int), typeof(MediaPlayerView),
                new PropertyMetadata(
                    (s, e) => { ((MediaPlayerView) s).OnStreamIndexProperty((int)e.NewValue, (int)e.OldValue); }));

        public static readonly DependencyProperty HasAudioOnlyProperty =
            DependencyProperty.Register("HasAudioOnly", typeof(bool), typeof(MediaPlayerView),
                new PropertyMetadata(false));

        public static readonly DependencyProperty PlaybackReadyTimeOutProperty =
            DependencyProperty.Register("PlaybackReadyTimeOut", typeof(int), typeof(MediaPlayerView),
                new PropertyMetadata(5));

        public static readonly DependencyProperty BufferingSecondsProperty =
            DependencyProperty.Register("BufferingSeconds", typeof(int), typeof(MediaPlayerView),
                new PropertyMetadata(5));

        public static readonly DependencyProperty TimeSeekerProperty =
            DependencyProperty.Register("TimeSeeker", typeof(MediaTimeSeeker), typeof(MediaPlayerView));

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(MediaPlayerView),
                new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsThumbnailVisibleProperty =
            DependencyProperty.Register("IsThumbnailVisible", typeof(bool), typeof(MediaPlayerView),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(MediaPlayerView),
                new FrameworkPropertyMetadata(false));


        public static readonly DependencyProperty HeaderDataProperty =
            DependencyProperty.Register("HeaderData", typeof(MediaHeaderData), typeof(MediaPlayerView));


        public MediaPlayerView()
        {
            InitializeComponent();

            Loaded +=
                (s, a) => { MessageCenter.Instance.Regist<MediaPlayer.PlayOrPauseMessage>(OnPlayOrPauseRequested); };

            Unloaded +=
                (s, a) => { MessageCenter.Instance.Unregist<MediaPlayer.PlayOrPauseMessage>(OnPlayOrPauseRequested); };
        }

        public IList<IText> SubtitleFlowDocumentText
        {
            get => (IList<IText>) GetValue(SubtitleFlowDocumentTextProperty);
            set => SetValue(SubtitleFlowDocumentTextProperty, value);
        }

        public bool IsPlaying
        {
            get => (bool) GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public MediaHeaderData HeaderData
        {
            get => (MediaHeaderData)GetValue(HeaderDataProperty);
            set => SetValue(HeaderDataProperty, value);
        }

        public bool IsThumbnailVisible
        {
            get => (bool) GetValue(IsThumbnailVisibleProperty);
            set => SetValue(IsThumbnailVisibleProperty, value);
        }

        public MediaTimeSeeker TimeSeeker
        {
            get => (MediaTimeSeeker) GetValue(TimeSeekerProperty);
            set => SetValue(TimeSeekerProperty, value);
        }

        public int PlaybackReadyTimeOut
        {
            get => (int) GetValue(PlaybackReadyTimeOutProperty);
            set => SetValue(PlaybackReadyTimeOutProperty, value);
        }

        public int BufferingSeconds
        {
            get => (int) GetValue(BufferingSecondsProperty);
            set => SetValue(BufferingSecondsProperty, value);
        }

        public decimal NaturalDuration
        {
            get => (decimal) GetValue(NaturalDurationProperty);
            set => SetValue(NaturalDurationProperty, value);
        }

        public decimal CurrentPosition
        {
            get => (decimal) GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public BitmapSource ThumbnailSource
        {
            get => (BitmapSource) GetValue(ThumbnailSourceProperty);
            set => SetValue(ThumbnailSourceProperty, value);
        }

        public string MediaSource
        {
            get => (string) GetValue(MediaSourceProperty);
            set => SetValue(MediaSourceProperty, value);
        }

        public int StreamIndex
        {
            get => (int) GetValue(StreamIndexProperty);
            set => SetValue(StreamIndexProperty, value);
        }

        public bool HasAudioOnly
        {
            get => (bool) GetValue(HasAudioOnlyProperty);
            set => SetValue(HasAudioOnlyProperty, value);
        }

        public double Volume
        {
            get => (double) GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        private double VolumeTemp { get; set; }

        private void OnMediaSourceProperty(string newValue, string oldValue)
        {
            if (!string.IsNullOrEmpty(oldValue))
                StopMedia();

            if (!string.IsNullOrEmpty(newValue))
                OpenMedia(newValue, StreamIndex);
        }

        private void OnStreamIndexProperty(int newValue, int oldValue)
        {
            StopMedia();

//            OpenMedia(MediaSource, newValue);
        }

        public event Action<decimal> OnPositionChanged;

        private void OnPlayOrPauseRequested(MediaPlayer.PlayOrPauseMessage message)
        {
            if (VideoElement.IsPlaying)
                PauseMedia();
            else
                PlayMedia(MediaSource, StreamIndex);
        }

        private void MediaPlayerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            AddEvent();
            IsThumbnailVisible = true;
            if (TimeSeeker is ITimeSeeker timeSeeker)
            {
                timeSeeker.Seek = positionSeconds =>
                {
                    if (positionSeconds < 0 || positionSeconds > NaturalDuration)
                        return;
                    VideoElement.Seek(positionSeconds);
                };
                VideoElement.SeekComplated += () => { timeSeeker.SeekComplatedAction?.Invoke(); };
            }
        }

        private void MediaPlayerView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            RemoveEvent();
        }

        private void RemoveEvent()
        {
            VideoElement.Failed -= OnFailed;
            VideoElement.OnPositionChanged -= VideoElement_OnPositionChanged;
            VideoElement.PlayStateChanged -= OnPlayStateChanged;
            MediaPlaybackButton.Checked -= MediaPlaybackButton_Checked;
            MediaPlaybackButton.Unchecked -= MediaPlaybackButton_Unchecked;
            MuteButton.Checked -= MuteButton_Checked;
            MuteButton.Unchecked -= MuteButton_Unchecked;
        }

        private void AddEvent()
        {
            VideoElement.Failed += OnFailed;
            VideoElement.OnPositionChanged += VideoElement_OnPositionChanged;
            VideoElement.PlayStateChanged += OnPlayStateChanged;
            MediaPlaybackButton.Checked += MediaPlaybackButton_Checked;
            MediaPlaybackButton.Unchecked += MediaPlaybackButton_Unchecked;
            MuteButton.Checked += MuteButton_Checked;
            MuteButton.Unchecked += MuteButton_Unchecked;
        }

        private void MuteButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Volume = VolumeTemp;
        }

        private void MuteButton_Checked(object sender, RoutedEventArgs e)
        {
            VolumeTemp = Volume;
            Volume = 0;
        }

        private void MediaPlaybackButton_Unchecked(object sender, RoutedEventArgs e)
        {
            PauseMedia();
        }

        private void MediaPlaybackButton_Checked(object sender, RoutedEventArgs e)
        {
            PlayMedia(MediaSource, StreamIndex);
        }

        private void OnPlayStateChanged(MediaPlayStates playState)
        {
            IsThumbnailVisible = playState == MediaPlayStates.Opened || 
                                 playState == MediaPlayStates.Manual ||
                                 playState == MediaPlayStates.Stop ||
                                 playState == MediaPlayStates.Closed;
        }

        private void VideoElement_OnPositionChanged(decimal value)
        {
            OnPositionChanged?.Invoke(value);
        }

        public event Action<MediaPlayStates> PlayStateChanged;

        private void VideoElement_OnPlayStateChanged(MediaPlayStates state)
        {
            MediaPlaybackButton.Checked -= MediaPlaybackButton_Checked;
            MediaPlaybackButton.Unchecked -= MediaPlaybackButton_Unchecked;
            switch (state)
            {
                case MediaPlayStates.Manual:
                    break;
                case MediaPlayStates.Opened:
                    HeaderData = VideoElement.MediaData;
                    break;
                case MediaPlayStates.Closed:
                    break;
                case MediaPlayStates.Buffering:
                    break;
                case MediaPlayStates.Seeking:
                    break;
                case MediaPlayStates.Play:
                    MediaPlaybackButton.IsChecked = true;
                    break;
                case MediaPlayStates.Pause:
                case MediaPlayStates.Stop:
                    MediaPlaybackButton.IsChecked = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            MediaPlaybackButton.Checked += MediaPlaybackButton_Checked;
            MediaPlaybackButton.Unchecked += MediaPlaybackButton_Unchecked;
            PlayStateChanged?.Invoke(state);
        }

        private void VideoElement_OnOnPositionChanged(decimal currentPosition)
        {
            CurrentPosition = currentPosition;
        }

        #region Media Control Func

        private void OpenMedia(string mediaSource = null, int streamIndex = 0)
        {
            var authorization = ((MediaPlayerViewModel)(this.DataContext)).getAuthorization();
            var projectId = ((MediaPlayerViewModel)(this.DataContext)).ProjectId;

            VideoElement.Source = mediaSource;
            VideoElement.Authorization = authorization;
            VideoElement.ProjectId = projectId;

            VideoElement.Open();
        }

        private void PlayMedia(string mediaSource = null, int streamIndex = 0)
        {
            var authorization = ((MediaPlayerViewModel) (this.DataContext)).getAuthorization();
            var projectId = ((MediaPlayerViewModel)(this.DataContext)).ProjectId;

            if (VideoElement == null)
                return;

            if (string.IsNullOrWhiteSpace(mediaSource))
                return;

            if (VideoElement.PlayState != MediaPlayStates.Pause)
            {
                if (VideoElement.IsPlaying)
                    VideoElement.Stop();

                if (string.IsNullOrEmpty(VideoElement.Source))
                    VideoElement.Source = mediaSource;
                else if (!VideoElement.Source.Equals(mediaSource)) VideoElement.Source = mediaSource;

                if (!VideoElement.StreamIndex.Equals(streamIndex)) VideoElement.StreamIndex = streamIndex;

                VideoElement.Authorization = authorization;
                VideoElement.ProjectId = projectId;
            }

            if (VideoElement.CanPlay)
                VideoElement.Play();
        }

        private void PauseMedia()
        {
            if (VideoElement.CanPause)
                VideoElement.Pause();
        }

        private void StopMedia()
        {
            if (VideoElement.CanStop)
                VideoElement.Stop();
        }

        private void OnFailed(Exception obj)
        {
            MediaPlaybackButton.Unchecked -= MediaPlaybackButton_Unchecked;
            MediaPlaybackButton.IsChecked = false;
            MediaPlaybackButton.Unchecked += MediaPlaybackButton_Unchecked;

            // 오류가 발생한 경우
            IsThumbnailVisible = false;
        }

        #endregion

        private void TypeUrlList_OnDropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox cb)
            {
                if (cb.Items.Count == 1)
                    cb.IsDropDownOpen = false;
            }
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Megazone.Core.Windows.Control.VideoPlayer;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     Interaction logic for MiniMediaPlayerView.xaml
    /// </summary>
    public partial class MiniMediaPlayerView : UserControl
    {
        public static readonly DependencyProperty NaturalDurationProperty =
            DependencyProperty.Register("NaturalDuration", typeof(decimal), typeof(MiniMediaPlayerView),
                new PropertyMetadata(0M));

        public static readonly DependencyProperty NaturalDurationVisibilityProperty =
            DependencyProperty.Register("NaturalDurationVisibility", typeof(Visibility), typeof(MiniMediaPlayerView),
                new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(MiniMediaPlayerView));

        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register("ThumbnailSource", typeof(BitmapSource), typeof(MiniMediaPlayerView));

        public static readonly DependencyProperty MediaSourceProperty =
            DependencyProperty.Register("MediaSource", typeof(string), typeof(MiniMediaPlayerView),
                new PropertyMetadata(
                    (s, e) => { ((MiniMediaPlayerView) s).OnMediaSourceProperty((string) e.OldValue); }));

        public static readonly DependencyProperty HasAudioOnlyProperty =
            DependencyProperty.Register("HasAudioOnly", typeof(bool), typeof(MiniMediaPlayerView),
                new PropertyMetadata(false));

        public static readonly DependencyProperty BufferingSecondsProperty =
            DependencyProperty.Register("BufferingSeconds",
                typeof(int), typeof(MiniMediaPlayerView), new PropertyMetadata(2));

        public static readonly DependencyProperty PlaybackReadyTimeOutProperty =
            DependencyProperty.Register("PlaybackReadyTimeOut",
                typeof(int), typeof(MiniMediaPlayerView), new PropertyMetadata(5));

        public MiniMediaPlayerView()
        {
            InitializeComponent();
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

        public Visibility NaturalDurationVisibility
        {
            get => (Visibility) GetValue(NaturalDurationVisibilityProperty);
            set => SetValue(NaturalDurationVisibilityProperty, value);
        }

        public string GroupName
        {
            get => (string) GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
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

        public bool HasAudioOnly
        {
            get => (bool) GetValue(HasAudioOnlyProperty);
            set => SetValue(HasAudioOnlyProperty, value);
        }

        public bool IsPlaying => VideoElement?.IsPlaying ?? false;

        public void Play()
        {
            PlayMedia(MediaSource);
        }

        public void Pause()
        {
            PauseMedia();
        }

        public void Stop()
        {
            StopMedia();
        }

        public void Seek(decimal seekSecconds)
        {
            VideoElement.Seek(seekSecconds);
        }

        public void SetVolume(double volume)
        {
            VideoElement.SetVolume(volume);
        }

        private void OnMediaSourceProperty(string oldValue)
        {
            if (!string.IsNullOrEmpty(oldValue))
                Stop();
        }

        private void MiniMediaPlayerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            AddEvent();
        }

        private void MiniMediaPlayerView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            RemoveEvent();
        }

        private void AddEvent()
        {
            VideoElement.PlayStateChanged += OnPlayStateChanged;
            VideoElement.Failed += OnFailed;
        }

        private void RemoveEvent()
        {
            VideoElement.PlayStateChanged -= OnPlayStateChanged;
            VideoElement.Failed -= OnFailed;

            MediaStopButton.Click -= MediaStopButton_OnClick;
            MediaPlayButton.Click -= MediaPlayButton_OnClick;
            MediaPlayButton.Unchecked -= MediaPlayButton_OnUnchecked;
        }

        private void MediaStopButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopMedia();
        }

        private void MediaPlayButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            StopMedia();
        }

        private void MediaPlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            PlayMedia(MediaSource);
        }

        public void Play(string mediaSource)
        {
            PlayMedia(mediaSource);
        }

        #region Media Control Func

        private void PlayMedia(string mediaSource = null)
        {
            if (VideoElement == null)
                return;

            if (string.IsNullOrWhiteSpace(mediaSource))
                return;

            if (VideoElement.IsPlaying)
                VideoElement.Stop();

            VideoElement.Source = mediaSource;
            VideoElement.Play();
        }

        private void PauseMedia()
        {
            if (VideoElement.IsPlaying)
                VideoElement.Pause();
        }

        private void StopMedia()
        {
            if (VideoElement.IsPlaying)
                VideoElement.Stop();
        }

        private void OnFailed(Exception obj)
        {
            if (MediaPlayButton.IsChecked == true)
                MediaPlayButton.IsChecked = false;
        }

        private void OnPlayStateChanged(MediaPlayStates playState)
        {
            if (playState == MediaPlayStates.Play)
                if (MediaPlayButton.IsChecked == false)
                    MediaPlayButton.IsChecked = true;

            if (playState == MediaPlayStates.Stop)
                if (MediaPlayButton.IsChecked == true)
                    MediaPlayButton.IsChecked = false;
        }

        #endregion
    }
}
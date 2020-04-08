using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Megazone.Core.Windows.Control.VideoPlayer;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class VideoElementSeekingBehavior : Behavior<Slider>
    {
        public static readonly DependencyProperty TargetVideoElementProperty =
            DependencyProperty.Register("TargetVideoElement", typeof(VideoElement),
                typeof(VideoElementSeekingBehavior));

        private readonly DispatcherTimer _renderTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromSeconds(0.02)
        };

        private Thumb _thumb;

        public VideoElement TargetVideoElement
        {
            get => (VideoElement) GetValue(TargetVideoElementProperty);
            set => SetValue(TargetVideoElementProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AddEvent();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            RemoveEvent();
        }

        private void AddEvent()
        {
            _renderTimer.Tick += _renderTimer_Tick;
            if (TargetVideoElement != null)
            {
                TargetVideoElement.PlayStateChanged += TargetVideoElement_PlayStateChanged;
                TargetVideoElement.SeekComplated += TargetVideoElement_SeekComplated;
            }

            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void TargetVideoElement_SeekComplated()
        {
            if (!_renderTimer.IsEnabled)
                AssociatedObject.Value = Convert.ToDouble(TargetVideoElement?.PositionSeconds ?? 0);
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.IsVisible)
                InitDragEventHandler();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            InitDragEventHandler();
        }

        private void InitDragEventHandler()
        {
            if (_thumb != null)
                return;

            var track = AssociatedObject.Template?.FindName("PART_Track", AssociatedObject) as Track;
            _thumb = track?.Thumb;

            if (_thumb != null)
            {
                _thumb.DragCompleted -= _thumb_DragCompleted;
                _thumb.DragStarted -= _thumb_DragStarted;
                _thumb.DragCompleted += _thumb_DragCompleted;
                _thumb.DragStarted += _thumb_DragStarted;
            }
        }

        private void RemoveEvent()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;

            if (_renderTimer.IsEnabled)
                _renderTimer.Stop();

            _renderTimer.Tick -= _renderTimer_Tick;

            if (TargetVideoElement != null)
            {
                TargetVideoElement.PlayStateChanged -= TargetVideoElement_PlayStateChanged;
                TargetVideoElement.SeekComplated -= TargetVideoElement_SeekComplated;
            }

            if (_thumb != null)
            {
                _thumb.DragCompleted -= _thumb_DragCompleted;
                _thumb.DragStarted -= _thumb_DragStarted;
            }
        }

        private void _renderTimer_Tick(object sender, EventArgs e)
        {
            if (AssociatedObject != null)
                AssociatedObject.Value = Convert.ToDouble(TargetVideoElement?.PositionSeconds ?? 0);
        }

        private void TargetVideoElement_PlayStateChanged(MediaPlayStates state)
        {
            switch (state)
            {
                case MediaPlayStates.Buffering:
                    StopRendering();
                    break;
                case MediaPlayStates.Opened:
                    StopRendering();
                    break;
                case MediaPlayStates.Closed:
                    StopRendering();
                    break;
                case MediaPlayStates.Play:
                    StartRendering();
                    break;
                case MediaPlayStates.Pause:
                    StopRendering();
                    break;
                case MediaPlayStates.Stop:
                    StopRendering();
                    AssociatedObject.Value = 0;
                    break;
            }

            AssociatedObject.IsEnabled = state == MediaPlayStates.Opened || 
                                         state ==  MediaPlayStates.Play || 
                                         state == MediaPlayStates.Pause;
        }

        private void StartRendering()
        {
            AssociatedObject.Maximum = Convert.ToDouble(TargetVideoElement?.NaturalDuration ?? 0);

            if (!_renderTimer.IsEnabled)
                _renderTimer.Start();
        }

        private void StopRendering()
        {
            if (_renderTimer.IsEnabled)
                _renderTimer.Stop();
        }

        private void _thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            StopRendering();
        }

        private void _thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (TargetVideoElement.PlayState == MediaPlayStates.Play)
                StartRendering();

            Seek(Convert.ToDecimal(AssociatedObject.Value));
        }

        private void Seek(decimal seekPosittion)
        {
            var dif = (TargetVideoElement?.PositionSeconds ?? 0) - seekPosittion;
            if (dif < 1 && dif >= 0)
                return;

            TargetVideoElement?.Seek(Convert.ToDecimal(AssociatedObject.Value));
        }
    }
}
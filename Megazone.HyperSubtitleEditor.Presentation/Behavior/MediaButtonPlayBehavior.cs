using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class MediaButtonPlayBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty MediaSourceProperty =
            DependencyProperty.Register("MediaSource", typeof(string), typeof(MediaButtonPlayBehavior));

        public static readonly DependencyProperty TargetMeidaControlProperty =
            DependencyProperty.Register("TargetMeidaControl",
                typeof(MiniMediaPlayerView),
                typeof(MediaButtonPlayBehavior));

        public string MediaSource
        {
            get => (string) GetValue(MediaSourceProperty);
            set => SetValue(MediaSourceProperty, value);
        }

        public MiniMediaPlayerView TargetMeidaControl
        {
            get => (MiniMediaPlayerView) GetValue(TargetMeidaControlProperty);
            set => SetValue(TargetMeidaControlProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AddEvent();
        }

        protected override void OnDetaching()
        {
            RemoveEvent();
            base.OnDetaching();
        }

        private void AddEvent()
        {
            AssociatedObject.Click += AssociatedObject_Click;
        }

        private void RemoveEvent()
        {
            AssociatedObject.Click -= AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            PlayMedia();
        }

        private void PlayMedia()
        {
            if (TargetMeidaControl == null)
                return;

            if (string.IsNullOrWhiteSpace(MediaSource))
                return;

            if (TargetMeidaControl.IsPlaying)
                TargetMeidaControl.Stop();

            TargetMeidaControl.Play(MediaSource);
        }
    }
}
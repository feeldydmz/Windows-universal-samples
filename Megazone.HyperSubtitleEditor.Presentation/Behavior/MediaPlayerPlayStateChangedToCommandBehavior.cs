using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class MediaPlayerPlayStateChangedToCommandBehavior : Behavior<MediaPlayerView>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(MediaPlayerPlayStateChangedToCommandBehavior));

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PlayStateChanged += AssociatedObject_PlayStateChanged;
        }

        private void AssociatedObject_PlayStateChanged(MediaPlayStates state)
        {
            Command?.Execute(state);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PlayStateChanged -= AssociatedObject_PlayStateChanged;
            base.OnDetaching();
        }
    }
}
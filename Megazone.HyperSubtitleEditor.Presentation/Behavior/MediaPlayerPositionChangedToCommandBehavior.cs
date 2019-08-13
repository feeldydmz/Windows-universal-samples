using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class MediaPlayerPositionChangedToCommandBehavior : Behavior<MediaPlayerView>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(MediaPlayerPositionChangedToCommandBehavior));

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.OnPositionChanged += AssociatedObject_OnPositionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.OnPositionChanged -= AssociatedObject_OnPositionChanged;
            base.OnDetaching();
        }

        private void AssociatedObject_OnPositionChanged(decimal value)
        {
            Command?.Execute(value);
        }
    }
}
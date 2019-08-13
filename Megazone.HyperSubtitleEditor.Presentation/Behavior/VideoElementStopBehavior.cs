using System.Windows;
using System.Windows.Interactivity;
using Megazone.Core.Windows.Control.VideoPlayer;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class VideoElementStopBehavior : Behavior<VideoElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            base.OnDetaching();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            StopMedia();
        }

        private void StopMedia()
        {
            if (AssociatedObject.IsPlaying)
                AssociatedObject.Stop();
        }
    }
}
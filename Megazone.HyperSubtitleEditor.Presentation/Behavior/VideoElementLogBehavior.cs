using System;
using System.Windows;
using System.Windows.Interactivity;
using Megazone.Core.Log;
using Megazone.Core.Windows.Control.VideoPlayer;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class VideoElementLogBehavior : Behavior<VideoElement>
    {
        private readonly ILogger _logger;

        public VideoElementLogBehavior()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            RemoveEvent();
            AddEvent();
        }

        protected override void OnDetaching()
        {
            RemoveEvent();

            base.OnDetaching();
        }

        private void AddEvent()
        {
            AssociatedObject.Unloaded += OnUnloaded;
            AssociatedObject.Failed += OnFailed;
        }

        private void RemoveEvent()
        {
            AssociatedObject.Unloaded -= OnUnloaded;
            AssociatedObject.Failed -= OnFailed;
        }


        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            RemoveEvent();
        }

        private void OnFailed(Exception exception)
        {
            _logger?.Error.Write(exception);
        }
    }
}
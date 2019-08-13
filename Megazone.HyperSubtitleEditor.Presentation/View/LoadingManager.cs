using System.Windows;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    internal class LoadingManager : LoadingManagerBase
    {
        private readonly FrameworkElement _view;

        public LoadingManager(FrameworkElement view)
        {
            _view = view;
        }

        protected override void HideView()
        {
            _view.Visibility = Visibility.Collapsed;
        }

        protected override void ShowView()
        {
            _view.Visibility = Visibility.Visible;
        }
    }
}
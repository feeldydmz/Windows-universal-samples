using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class PopupHeightControlBehavior : Behavior<Popup>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Opened += AssociatedObject_Opened;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Opened -= AssociatedObject_Opened;
        }

        private void AssociatedObject_Opened(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow != null)
                if (sender is Popup popup)
                    popup.Height = (Application.Current.MainWindow.Content as FrameworkElement)?.ActualHeight - 1 ?? 0d;
        }
    }
}
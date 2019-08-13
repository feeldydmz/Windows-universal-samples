using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using Megazone.Core.Windows.Extension;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class CheckedToFocusBehavior : Behavior<ToggleButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Checked += AssociatedObject_Checked;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Checked -= AssociatedObject_Checked;
            base.OnDetaching();
        }

        private void AssociatedObject_Checked(object sender, RoutedEventArgs e)
        {
            this.InvokeOnUi(() => { AssociatedObject.Focus(); });
        }
    }
}
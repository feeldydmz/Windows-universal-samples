using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ListboxItemDoubleClickBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand),
            typeof(ListboxItemDoubleClickBehavior));

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseDoubleClick += AssociatedObjectOnPreviewMouseDoubleClick;
        }

        private void AssociatedObjectOnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Command?.Execute("");
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDoubleClick -= AssociatedObjectOnPreviewMouseDoubleClick;
        }
    }
}
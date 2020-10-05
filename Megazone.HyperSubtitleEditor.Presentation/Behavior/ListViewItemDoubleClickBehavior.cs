using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.Core.Windows.Extension;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class ListViewItemDoubleClickBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand),
            typeof(ListViewItemDoubleClickBehavior));

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
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
            AssociatedObject.Unloaded += OnUnloaded;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void RemoveEvent()
        {
            AssociatedObject.Unloaded -= OnUnloaded;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = e.OriginalSource as DependencyObject;

            var listViewItem = dependencyObject?.FindParentElement<ListViewItem>();
            if (listViewItem == null)
                return;

            // 더블클릭
            if (e.ClickCount >= 2)
                Command.Execute(null);

            // 마우스 이벤트를 MainWindow로 전달
            var arg = new MouseButtonEventArgs(
                Mouse.PrimaryDevice, 0, MouseButton.Left | MouseButton.Right);
            arg.RoutedEvent = UIElement.MouseDownEvent;

            Application.Current?.MainWindow?.RaiseEvent(arg);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            RemoveEvent();
        }
    }
}
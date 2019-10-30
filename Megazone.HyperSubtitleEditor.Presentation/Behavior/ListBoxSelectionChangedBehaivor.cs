using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

//using Megazone.Core.Windows.Xaml.Behaviors.Primitives;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ListBoxSelectionChangedBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ListBoxSelectionChangedBehavior));

        private static ListBox _currentListBox;


        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (_currentListBox != null)
                if (listBox != null && !_currentListBox.Uid.Equals(listBox.Uid))
                    _currentListBox.SelectedItem = null;

            _currentListBox = listBox;


            Command?.Execute("");
        }
    }
}
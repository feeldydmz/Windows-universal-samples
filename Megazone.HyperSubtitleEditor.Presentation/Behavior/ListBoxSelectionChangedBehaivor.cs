using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

//using Megazone.Core.Windows.Xaml.Behaviors.Primitives;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ListBoxSelectionChangedBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ListBoxSelectionChangedBehavior));


        private SignInViewModel SigninViewmodel { get; set; }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            SigninViewmodel = Bootstrapper.Container.Resolve<SignInViewModel>();

            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
        }

        private static ListBox _currentListBox;

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (_currentListBox != null)
            {
                if (listBox != null && !_currentListBox.Uid.Equals(listBox.Uid))
                {
                    _currentListBox.SelectedItem = null;
                }
            }

            _currentListBox = listBox;


            Command?.Execute("");
        }
    }
}

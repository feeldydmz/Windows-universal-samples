using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Input;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class LeftSideRadioMenuButtonBehavior : Behavior<RadioButton>
    {
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand),
                typeof(LeftSideRadioMenuButtonBehavior));

        public ICommand CloseCommand
        {
            get => (ICommand) GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Unchecked += AssociatedObjectOnUnchecked;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Unchecked -= AssociatedObjectOnUnchecked;
        }

        private void AssociatedObjectOnUnchecked(object sender, RoutedEventArgs e)
        {
            CloseCommand?.Execute("");
        }
    }
}

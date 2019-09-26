using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.Core.Windows.Extension;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ListBoxDataLoadBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",typeof(ICommand),typeof(ListBoxDataLoadBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ListBoxDataLoadBehavior));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
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
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        private void RemoveEvent()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= _scrollViewer_ScrollChanged;
            }
        }

        private ScrollViewer _scrollViewer;
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = AssociatedObject.FindChildElement<ScrollViewer>(true);
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= _scrollViewer_ScrollChanged;
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
            }
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var isEnd = false;

            if (e.ExtentHeight == e.VerticalOffset + e.ViewportHeight)
            {
                isEnd = true;
            }

            if (isEnd)
            {
                System.Diagnostics.Debug.WriteLine("더 불러오기.");
                Command?.Execute(CommandParameter);
            }
        }
    }
}

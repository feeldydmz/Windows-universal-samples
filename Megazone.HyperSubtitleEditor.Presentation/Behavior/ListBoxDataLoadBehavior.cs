﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.Core.Windows.Extension;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ListBoxDataLoadBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ListBoxDataLoadBehavior));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ListBoxDataLoadBehavior));

        private DateTime _callTime;

        private ScrollViewer _scrollViewer;

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

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
        }

        private void RemoveEvent()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;

            if (_scrollViewer != null)
                _scrollViewer.ScrollChanged -= _scrollViewer_ScrollChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = AssociatedObject.FindChildElement<ScrollViewer>(true);
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= _scrollViewer_ScrollChanged;
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
            }
        }

        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (DateTime.Now.Subtract(_callTime).TotalMilliseconds > 100)
            {
                _callTime = DateTime.Now;
                Debug.WriteLine(
                    $"{e.ExtentHeight - (e.VerticalOffset + e.ViewportHeight)}, VerticalChange: {e.VerticalChange}");
                if (e.ExtentHeight > 0 && e.VerticalOffset > 0)
                    if (e.ExtentHeight - (e.VerticalOffset + e.ViewportHeight + e.VerticalChange) < 2)
                        Command?.Execute(CommandParameter);
            }
        }
    }
}
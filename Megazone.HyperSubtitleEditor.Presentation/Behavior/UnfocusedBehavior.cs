using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class UnfocusedBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(UnfocusedBehavior),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (s, e) => { ((UnfocusedBehavior) s).OnIsOpenPropertyChanged((bool) e.NewValue); }));

        public bool IsVisible
        {
            get => (bool) GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        private void OnIsOpenPropertyChanged(bool newValue)
        {
            if (AssociatedObject != null)
                AssociatedObject.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Application.Current.MainWindow.StateChanged -= MainWindow_StateChanged;
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
            Application.Current.MainWindow.LocationChanged -= MainWindow_LocationChanged;
            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            Application.Current.MainWindow.MouseDown -= MainWindow_MouseDown;
            Application.Current.MainWindow.MouseDown += MainWindow_MouseDown;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsVisible = false;
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            IsVisible = false;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            IsVisible = false;
        }

        private void AssociatedObjectOnGotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("AssociatedObjectOnGotFocus");
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            //AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
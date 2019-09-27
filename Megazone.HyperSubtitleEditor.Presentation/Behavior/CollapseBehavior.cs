using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    // Left Side Menu bar를 숨기기 위해 만들었는데..
    public class CollapseBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(CollapseBehavior),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (s, e) => { ((CollapseBehavior) s).OnIsOpenPropertyChanged((bool) e.NewValue); }));

        public bool IsVisible
        {
            get => (bool) GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        private void OnIsOpenPropertyChanged(bool newValue)
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Loaded += AssociatedObject_Loaded;

            if (Application.Current.MainWindow == null)
                return;

            Application.Current.MainWindow.Deactivated -= MainWindow_Deactivated;
            Application.Current.MainWindow.Deactivated += MainWindow_Deactivated;
            Application.Current.MainWindow.StateChanged -= MainWindow_StateChanged;
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
            Application.Current.MainWindow.LocationChanged -= MainWindow_LocationChanged;
            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            Application.Current.MainWindow.MouseDown -= MainWindow_MouseDown;
            Application.Current.MainWindow.MouseDown += MainWindow_MouseDown;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsVisible)
                return;

            var rect = new Rect(new Point(0, 0),
                new Point(AssociatedObject.ActualWidth, AssociatedObject.ActualHeight));
            if (!rect.Contains(e.GetPosition(AssociatedObject)))
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

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            IsVisible = false;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            OnIsOpenPropertyChanged(IsVisible);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
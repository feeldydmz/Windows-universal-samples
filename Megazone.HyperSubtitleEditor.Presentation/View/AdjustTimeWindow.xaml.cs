using System;
using System.Windows;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     AdjustTimeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AdjustTimeWindow : Window
    {
        public AdjustTimeWindow()
        {
            InitializeComponent();

            Loaded += AdjustTimeWindow_Loaded;
            Unloaded += AdjustTimeWindow_Unloaded;
        }

        public TimeSpan Time { get; private set; }

        public AdjustTimeRange Range { get; private set; }

        public AdjustTimeBehavior Behavior { get; private set; }

        private void AdjustTimeWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            TimeSpinner.TimeChanged -= TimeSpinner_TimeChanged;
        }

        private void AdjustTimeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSpinner.TimeChanged += TimeSpinner_TimeChanged;
        }

        private void TimeSpinner_TimeChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            FowardButton.IsEnabled = BackwardButton.IsEnabled = TimeSpinner.Time > TimeSpan.Zero;
        }

        private AdjustTimeRange GetRange()
        {
            if (AllRangeRadioButton.IsChecked != null && AllRangeRadioButton.IsChecked.Value)
                return AdjustTimeRange.All;
            if (SelectedItemsRangeRadioButton.IsChecked != null && SelectedItemsRangeRadioButton.IsChecked.Value)
                return AdjustTimeRange.SelectedItems;
            return AdjustTimeRange.SelectedItemsAndBackItems;
        }

        private void FowardButton_OnClick(object sender, RoutedEventArgs e)
        {
            Behavior = AdjustTimeBehavior.Foward;
            Time = TimeSpinner.Time;
            Range = GetRange();
            Close();
        }

        private void BackwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            Behavior = AdjustTimeBehavior.Backward;
            Time = TimeSpinner.Time;
            Range = GetRange();
            Close();
        }
    }
}
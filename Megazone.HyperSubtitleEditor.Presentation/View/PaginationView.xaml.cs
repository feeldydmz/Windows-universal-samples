using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     PaginationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PaginationView : UserControl
    {
        public static readonly DependencyProperty PageItemsProperty =
            DependencyProperty.Register("PageItems", typeof(IEnumerable<int>), typeof(PaginationView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(PaginationView), new PropertyMetadata(0,
                (s, e) => { ((PaginationView) s).OnTotalCountChanged((int) e.NewValue); }));

        public static readonly DependencyProperty CountPerPageProperty =
            DependencyProperty.Register("CountPerPage", typeof(int), typeof(PaginationView), new PropertyMetadata(0,
                (s, e) => { ((PaginationView) s).OnCountPerPageChanged((int) e.NewValue); }));

        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PaginationView), new PropertyMetadata(0));

        public static readonly DependencyProperty DisplayCountProperty =
            DependencyProperty.Register("DisplayCount", typeof(int), typeof(PaginationView), new PropertyMetadata(5));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(PaginationView),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (s, e) => { ((PaginationView) s).OnValueChanged((int) e.OldValue, (int) e.NewValue); }));

        public static readonly DependencyProperty IsPreviousDotVisibleProperty =
            DependencyProperty.Register("IsPreviousDotVisible", typeof(bool), typeof(PaginationView),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsNextDotVisibleProperty =
            DependencyProperty.Register("IsNextDotVisible", typeof(bool), typeof(PaginationView),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(PaginationView));

        public PaginationView()
        {
            InitializeComponent();
        }

        public IEnumerable<int> PageItems
        {
            get => GetValue(PageItemsProperty) as IEnumerable<int>;
            set => SetValue(PageItemsProperty, value);
        }

        public int TotalCount
        {
            get => (int) GetValue(TotalCountProperty);
            set => SetValue(TotalCountProperty, value);
        }

        public int CountPerPage
        {
            get => (int) GetValue(CountPerPageProperty);
            set => SetValue(CountPerPageProperty, value);
        }

        public int PageCount
        {
            get => (int) GetValue(PageCountProperty);
            set => SetValue(PageCountProperty, value);
        }

        public int DisplayCount
        {
            get => (int) GetValue(DisplayCountProperty);
            set => SetValue(DisplayCountProperty, value);
        }

        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public int Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsPreviousDotVisible
        {
            get => (bool) GetValue(IsPreviousDotVisibleProperty);
            set => SetValue(IsPreviousDotVisibleProperty, value);
        }

        public bool IsNextDotVisible
        {
            get => (bool) GetValue(IsNextDotVisibleProperty);
            set => SetValue(IsNextDotVisibleProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<int> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Value > 1)
                Value--;
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (PageCount > Value)
                Value++;
        }

        private void OnValueChanged(int oldValue, int newValue)
        {
            PreviousButton.IsEnabled = newValue > 1;
            NextButton.IsEnabled = newValue < PageCount;

            if (PageItems != null && !PageItems.Any(pageNo => pageNo.Equals(newValue)))
                UpdatePageItems(CountPerPage, TotalCount, newValue);

            if (oldValue != newValue)
            {
                var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue, ValueChangedEvent) {Source = this};
                RaiseEvent(e);
            }
        }

        private void OnTotalCountChanged(int totalCount)
        {
            UpdatePageItems(CountPerPage, totalCount, Value);
        }

        private void OnCountPerPageChanged(int countPerPage)
        {
            UpdatePageItems(countPerPage, TotalCount, Value);
        }

        private void UpdatePageItems(int countPerPage, int totalCount, int value)
        {
            if (countPerPage > 0 && totalCount > 0)
            {
                var pageCount = totalCount / countPerPage;
                var rest = totalCount % countPerPage;
                if (rest > 0)
                    pageCount++;
                PageCount = pageCount;
                var displayedItemCount = DisplayCount > pageCount ? pageCount : DisplayCount;
                var start = 0;
                var end = displayedItemCount;
                if (DisplayCount < pageCount)
                    if (value > end)
                    {
                        start = value / DisplayCount * DisplayCount;
                        end = start + DisplayCount;
                        if (end > pageCount)
                            end = pageCount;
                    }

                var list = new List<int>();
                for (var index = start; end > index; index++)
                    list.Add(index + 1);

                PageItems = new ObservableCollection<int>(list);
                if (value == 0)
                    Value = 1;

                IsPreviousDotVisible = start > 1;
                IsNextDotVisible = pageCount - end > DisplayCount;
            }
        }
    }
}
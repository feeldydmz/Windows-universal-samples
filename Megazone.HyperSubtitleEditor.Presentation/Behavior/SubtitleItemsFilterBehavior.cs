using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class SubtitleItemsFilterBehavior : Behavior<ItemsControl>
    {
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register("FilterText",
            typeof(string),
            typeof(SubtitleItemsFilterBehavior),
            new FrameworkPropertyMetadata("",
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (s, e) => { (s as SubtitleItemsFilterBehavior)?.OnFilterTextChanged((string) e.NewValue); })
        );

        public string FilterText
        {
            get => (string) GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AddEvent();
        }

        protected override void OnDetaching()
        {
            RemoveEvent();
            base.OnDetaching();
        }

        private void AddEvent()
        {
        }

        private void RemoveEvent()
        {
        }

        private void OnFilterTextChanged(string newFilterText)
        {
            if (AssociatedObject == null)
                return;

            AssociatedObject.Items.Filter = item =>
            {
                return !(item is SubtitleListItemViewModel subtitleItem) || IsContain(subtitleItem, newFilterText);
            };
        }

        private bool IsContain(SubtitleListItemViewModel subtitleItem, string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return true;
            var text = subtitleItem.DisplayText ?? string.Empty;
            return text.ToLower()
                .Contains(filter.ToLower());
        }
    }
}
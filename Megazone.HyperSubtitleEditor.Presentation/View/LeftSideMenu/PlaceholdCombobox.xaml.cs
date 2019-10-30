using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View.LeftSideMenu
{
    /// <summary>
    ///     PlaceholdCombobox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlaceholdCombobox : UserControl
    {
        public static DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(PlaceholdCombobox));

        public static DependencyProperty MyItemsSourceProperty =
            DependencyProperty.Register("MyItemsSource", typeof(IEnumerable), typeof(PlaceholdCombobox));

        public static DependencyProperty MySelectedItemProperty =
            DependencyProperty.Register("MySelectedItem", typeof(object), typeof(PlaceholdCombobox));

        public static DependencyProperty MyDataTemplateProperty =
            DependencyProperty.Register("MyDataTemplate", typeof(DataTemplate), typeof(PlaceholdCombobox));

        public static DependencyProperty MyDataTemplateSelectorProperty =
            DependencyProperty.Register("MyDataTemplateSelector", typeof(DataTemplateSelector),
                typeof(PlaceholdCombobox));

        public PlaceholdCombobox()
        {
            InitializeComponent();
        }

        public string DefaultText
        {
            get => (string) GetValue(DefaultTextProperty);
            set => SetValue(DefaultTextProperty, value);
        }

        public IEnumerable MyItemsSource
        {
            get => (IEnumerable) GetValue(MyItemsSourceProperty);
            set => SetValue(MyItemsSourceProperty, value);
        }

        public object MySelectedItem
        {
            get => GetValue(MySelectedItemProperty);
            set => SetValue(MySelectedItemProperty, value);
        }

        public DataTemplate MyDataTemplate
        {
            get => (DataTemplate) GetValue(MyDataTemplateProperty);
            set => SetValue(MyDataTemplateProperty, value);
        }

        public DataTemplateSelector MyDataTemplateSelector
        {
            get => (DataTemplateSelector) GetValue(MyDataTemplateProperty);
            set => SetValue(MyDataTemplateProperty, value);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Megazone.HyperSubtitleEditor.Presentation.View.LeftSideMenu
{
    /// <summary>
    /// PlaceholdCombobox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlaceholdCombobox : UserControl
    {
        public PlaceholdCombobox()
        {
            InitializeComponent();
        }

        public static DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(PlaceholdCombobox));

        public static DependencyProperty MyItemsSourceProperty =
            DependencyProperty.Register("MyItemsSource", typeof(IEnumerable), typeof(PlaceholdCombobox));

        public static DependencyProperty MySelectedItemProperty =
            DependencyProperty.Register("MySelectedItem", typeof(object), typeof(PlaceholdCombobox));

        public static DependencyProperty MyDataTemplateProperty =
            DependencyProperty.Register("MyDataTemplate", typeof(DataTemplate), typeof(PlaceholdCombobox));

        public static DependencyProperty MyDataTemplateSelectorProperty =
            DependencyProperty.Register("MyDataTemplateSelector", typeof(DataTemplateSelector), typeof(PlaceholdCombobox));

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        public IEnumerable MyItemsSource
        {
            get { return (IEnumerable)GetValue(MyItemsSourceProperty); }
            set { SetValue(MyItemsSourceProperty, value); }
        }

        public object MySelectedItem
        {
            get { return GetValue(MySelectedItemProperty); }
            set { SetValue(MySelectedItemProperty, value); }
        }

        public DataTemplate MyDataTemplate
        {
            get { return (DataTemplate)GetValue(MyDataTemplateProperty); }
            set { SetValue(MyDataTemplateProperty, value); }
        }

        public DataTemplateSelector MyDataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MyDataTemplateProperty); }
            set { SetValue(MyDataTemplateProperty, value); }
        }
    }
}

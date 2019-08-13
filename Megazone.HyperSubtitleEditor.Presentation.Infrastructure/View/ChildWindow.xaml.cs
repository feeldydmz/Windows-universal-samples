using System.Windows;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View
{
    /// <summary>
    ///     Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        public ChildWindow()
        {
            InitializeComponent();
        }

        public ChildWindow(FrameworkElement childView)
        {
            InitializeComponent();
            ContentContainer.Child = childView;
        }

        public void AddSubView(FrameworkElement element)
        {
            ContentContainer.Child = element;
        }

        public void RemoveSubView(FrameworkElement element)
        {
            ContentContainer.Child = null;
        }
    }
}
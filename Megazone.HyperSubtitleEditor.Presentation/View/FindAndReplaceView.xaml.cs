using System.Windows;
using System.Windows.Controls;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     FindAndReplaceView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FindAndReplaceView : UserControl
    {
        public FindAndReplaceView()
        {
            InitializeComponent();

            Loaded += FindAndReplaceView_Loaded;
        }

        internal FindAndReplaceViewModel ViewModel => DataContext as FindAndReplaceViewModel;

        private void FindAndReplaceView_Loaded(object sender, RoutedEventArgs e)
        {
            FindTextBox.Focus();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = this.FindParentElement<Window>();
            window?.Close();
        }
    }
}
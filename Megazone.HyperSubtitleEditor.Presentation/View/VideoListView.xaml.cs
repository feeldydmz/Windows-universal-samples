using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    /// VideoListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VideoListView : UserControl
    {
        public VideoListView()
        {
            InitializeComponent();
            Loaded += (s, e) => {
                if (DataContext is VideoListViewModel viewModel)
                    viewModel.CloseAction = CloseWindow;
            };
            Unloaded += (s, e) => {
                if (DataContext is VideoListViewModel viewModel)
                    viewModel.CloseAction = null;
            };
        }

        private void CancelButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}

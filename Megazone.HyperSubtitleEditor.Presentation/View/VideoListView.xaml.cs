using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     VideoListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VideoListView : UserControl
    {
        public VideoListView()
        {
            InitializeComponent();
            Loaded += VideoListView_Loaded;
            Unloaded += VideoListView_Unloaded;
        }

        private void VideoListView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is VideoListViewModel viewModel)
                viewModel.CloseAction = null;
        }

        private void VideoListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is VideoListViewModel viewModel)
                viewModel.CloseAction = CloseWindow;
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
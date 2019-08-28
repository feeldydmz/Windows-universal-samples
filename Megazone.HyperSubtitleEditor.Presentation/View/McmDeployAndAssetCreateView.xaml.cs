using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     McmDeployAndAssetCreateView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class McmDeployAndAssetCreateView : UserControl
    {
        public McmDeployAndAssetCreateView()
        {
            InitializeComponent();
            Loaded += McmDeployAndAssetCreateView_Loaded;
            Unloaded += McmDeployAndAssetCreateView_Unloaded;
        }

        private void McmDeployAndAssetCreateView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is McmDeployViewModel viewModel)
                viewModel.CloseAction = null;
        }

        private void McmDeployAndAssetCreateView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is McmDeployViewModel viewModel)
                viewModel.CloseAction = CloseWindow;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
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
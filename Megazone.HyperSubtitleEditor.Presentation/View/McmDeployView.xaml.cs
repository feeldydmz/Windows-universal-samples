using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     McmDeployView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class McmDeployView : UserControl
    {
        public McmDeployView()
        {
            InitializeComponent();
            Loaded += McmDeployView_Loaded;
            Unloaded += McmDeployView_Unloaded;
        }

        private void McmDeployView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is McmDeployViewModel viewModel)
                viewModel.CloseAction = null;
        }

        private void McmDeployView_Loaded(object sender, RoutedEventArgs e)
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
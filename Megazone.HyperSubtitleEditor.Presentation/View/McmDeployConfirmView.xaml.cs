using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    public partial class McmDeployConfirmView : UserControl
    {
        public McmDeployConfirmView()
        {
            InitializeComponent();
            Loaded += McmDeployConfirmView_Loaded;
            Unloaded += McmDeployConfirmView_Unloaded;
        }

        private void McmDeployConfirmView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is McmDeployConfirmViewModel viewModel)
                viewModel.CloseAction = null;
        }

        private void McmDeployConfirmView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is McmDeployConfirmViewModel viewModel)
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

using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    public partial class McmDeployConfirmView : UserControl
    {
        public McmDeployConfirmView()
        {
            InitializeComponent();
        }

        private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}
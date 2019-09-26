using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Megazone.HyperSubtitleEditor.Presentation.View.LeftSideMenu
{
    /// <summary>
    ///     LeftSideMenuView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LeftSideMenuView : UserControl
    {
        public LeftSideMenuView()
        {
            InitializeComponent();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            McmRadioButton.IsChecked = false;
            MyComputerRadioButton.IsChecked = false;
            RecentlyRadioButton.IsChecked = false;
        }
    }
}
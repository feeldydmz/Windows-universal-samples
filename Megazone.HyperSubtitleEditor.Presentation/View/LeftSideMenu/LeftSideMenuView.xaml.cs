using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
            IsVisibleChanged += (s, e) =>
            {
                McmRadioButton.IsChecked = false;
                RecentlyRadioButton.IsChecked = false;
                MyComputerRadioButton.IsChecked = false;
            };
        }
        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            McmRadioButton.IsChecked = false;
            MyComputerRadioButton.IsChecked = false;
            RecentlyRadioButton.IsChecked = false;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                //radioButton.IsChecked = true;
            }
        }

        private void McmRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (VideoRadioButton.IsChecked != true)
            {
                VideoRadioButton.IsChecked = true;
                VideoRadioButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}
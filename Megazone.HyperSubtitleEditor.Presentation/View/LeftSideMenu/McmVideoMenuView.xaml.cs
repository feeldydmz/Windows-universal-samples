using System;
using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View.LeftSideMenu
{
    /// <summary>
    ///     McmVideoMenuView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class McmVideoMenuView : UserControl
    {
        public McmVideoMenuView()
        {
            InitializeComponent();

            Loaded += McmVideoMenuView_Loaded;
        }

        private void McmVideoMenuView_Loaded(object sender, RoutedEventArgs e)
        {
            //if (DataContext is VideoListViewModel vm)
            //{
            //    vm.OnLoadAction = () => { CaptionBackButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); };

            //    vm.CloseAction = () => { CaptionBackButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); };
            //}
        }

        private void McmVideoMenuView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is VideoListViewModel vm) vm.Close();
        }

        private void McmVideoMenuView_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Console.Write("DD");
        }
    }
}
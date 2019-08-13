using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     AddSubtitleView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OpenSubtitleView : UserControl
    {
        public OpenSubtitleView()
        {
            InitializeComponent();
            Loaded += OpenSubtitleView_Loaded;
            Unloaded += OpenSubtitleView_Unloaded;
        }

        private void OpenSubtitleView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpenSubtitleViewModel vm)
                vm.CloseAction = CloseWindow;
        }

        private void OpenSubtitleView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpenSubtitleViewModel vm)
                vm.CloseAction = null;
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
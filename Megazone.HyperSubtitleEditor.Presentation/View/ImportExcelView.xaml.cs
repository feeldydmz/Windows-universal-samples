using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     ImportExcelView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImportExcelView : UserControl
    {
        public ImportExcelView()
        {
            InitializeComponent();
            Loaded += ImportExcelView_Loaded;
            Unloaded += ImportExcelView_Unloaded;
        }

        private void ImportExcelView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImportExcelViewModel vm)
                vm.CloseAction = null;
        }

        private void ImportExcelView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImportExcelViewModel vm)
                vm.CloseAction = CloseWindow;
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
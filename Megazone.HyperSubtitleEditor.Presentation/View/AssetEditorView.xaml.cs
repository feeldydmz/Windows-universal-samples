using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    /// AssetEditorView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AssetEditorView : UserControl
    {
        public AssetEditorView()
        {
            InitializeComponent();

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

        private void AssetEditorView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssetEditorViewModel viewModel)
                viewModel.CloseAction = CloseWindow;

            AssetNameTextBox.Focus();

            if (!string.IsNullOrEmpty(AssetNameTextBox.Text))
                AssetNameTextBox.CaretIndex = AssetNameTextBox.Text.Length;
        }

        private void AssetEditorView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssetEditorViewModel viewModel)
                viewModel.CloseAction = null;
        }
    }
}

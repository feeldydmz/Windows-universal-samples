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

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    /// CreateWorksapceConfirmView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CreateWorksapceConfirmView : Window
    {
        public bool IsCreateNewWindow { get; set; } = false;

        public CreateWorksapceConfirmView()
        {
            InitializeComponent();
        }

        private void ConfirmButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            IsCreateNewWindow = YesButton.IsChecked == true;

            CloseWindow();
        }
        
        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}

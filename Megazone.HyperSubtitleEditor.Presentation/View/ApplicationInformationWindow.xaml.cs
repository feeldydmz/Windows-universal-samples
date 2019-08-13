using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     ApplicationInformationWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ApplicationInformationWindow : Window
    {
        public ApplicationInformationWindow()
        {
            InitializeComponent();
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenCompanyURLButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://cloud.hosting.kr/");
        }
    }
}
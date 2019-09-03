using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     ProjectSelectView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectSelectView : UserControl
    {
        public ProjectSelectView()
        {
            InitializeComponent();
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
                e.Handled = true;
            }
            catch
            {
                // ignored
            }
        }
    }
}
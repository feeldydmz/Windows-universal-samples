using System.Windows.Controls;

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
            this.Initialized += McmVideoMenuView_Initialized;
        }

        private void McmVideoMenuView_Initialized(object sender, System.EventArgs e)
        {
            
        }
    }
}
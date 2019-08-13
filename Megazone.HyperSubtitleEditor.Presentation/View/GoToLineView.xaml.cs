using System.Windows;
using System.Windows.Controls;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     GoToLineView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GoToLineView : UserControl
    {
        public GoToLineView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}
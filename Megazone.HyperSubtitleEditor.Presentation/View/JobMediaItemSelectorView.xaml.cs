using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     Interaction logic for JobMediaItemSelectorView.xaml
    /// </summary>
    public partial class JobMediaItemSelectorView : UserControl
    {
        private ILoadingManager _loadingManager;

        public JobMediaItemSelectorView()
        {
            InitializeComponent();
        }

        public ILoadingManager LoadingManager
        {
            get { return _loadingManager = _loadingManager ?? new LoadingManager(LoadingProgressView); }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.Browser;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.View;

namespace Megazone.AtomyExcelConverter.Presentation.View
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : UserControl, IMainView
    {
        private ILoadingManager _loadingManager;

        public MainView()
        {
            Self = this;
            InitializeComponent();
        }

        internal static IMainView Self { get; private set; }

        public ILoadingManager LoadingManager
        {
            get { return _loadingManager = _loadingManager ?? new LoadingManager(LoadingProgressView); }
        }
    }
}

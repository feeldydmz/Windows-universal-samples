using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View.LeftSideMenu
{
    /// <summary>
    /// CaptionAssetListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CaptionAssetListView : UserControl
    {
        internal static readonly DependencyProperty CaptionAssetListProperty  =
            DependencyProperty.Register("CaptionAssetList", typeof(IEnumerable<CaptionAssetItemViewModel>), typeof(CaptionAssetListView), new PropertyMetadata(
                null,
                (s, a) =>
                    ((CaptionAssetListView)s).OnCaptionAssetListChanged()));

        internal static readonly DependencyProperty SelectedCaptionAssetProperty =
            DependencyProperty.Register("SelectedCaptionAsset", typeof(CaptionAssetItemViewModel), typeof(CaptionAssetListView));

        internal static readonly DependencyProperty ChangedListSelectionProperty =
            DependencyProperty.Register("ChangedListSelection", typeof(ICommand), typeof(CaptionAssetListView));

        internal IEnumerable<CaptionAssetItemViewModel> CaptionAssetList
        {
            get => (IEnumerable<CaptionAssetItemViewModel>) GetValue(CaptionAssetListProperty);
            set => SetValue(CaptionAssetListProperty, value);
        }

        internal CaptionAssetItemViewModel SelectedCaptionAsset
        {
            get => (CaptionAssetItemViewModel)GetValue(SelectedCaptionAssetProperty);
            set => SetValue(SelectedCaptionAssetProperty, value);
        }

        internal ICommand ChangedListSelection
        {
            get => (ICommand)GetValue(ChangedListSelectionProperty);
            set => SetValue(ChangedListSelectionProperty, value);
        }

        private void OnCaptionAssetListChanged()
        {
            Debug.WriteLine("OnCaptionAssetListChanged");

        }

        public CaptionAssetListView()
        {
            InitializeComponent();
        }
    }
}

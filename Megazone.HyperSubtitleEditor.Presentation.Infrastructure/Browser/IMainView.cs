using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser
{
    public interface IMainView
    {
        ILoadingManager LoadingManager { get; }
        ISubtitleView SubtitleView { get; }
        void ShowSettingsDialog();
        void ShowImportExcelDialog(string initialFilePath = null);
        void ShowOpenSubtitleDialog(string initialFilePath = null);
        void ShowAddAndEditSubtitleDialog(SubtitleDialogViewMode mode, ISubtitleTabItemViewModel tabItem, string title);
        void ShowCopySubtitleDialog(IEnumerable<ISubtitleTabItemViewModel> tabs);
        void ShowFindDialog();
        void ShowFindAndReplaceDialog();
        AdjustTimeWay ShowAdjustTimeWindow();
        void ShowOpenVideoAddressWindow();
        void ShowApplicationInfoWindow();
        void ShowVideoListDialog();
        void ShowMcmDeployAndAssetCreateDialog();
        void ShowMcmDeployDialog();
        void ShowMcmDeployConfirmDialog(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions, string linkUrl);
        void SetWindowTitle(string title);
        void ShowAssetEditorDialog(bool isNewCreateMode, string assetName = null);
    }
}
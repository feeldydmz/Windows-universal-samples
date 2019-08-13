﻿using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser
{
    public interface IMainView
    {
        ILoadingManager LoadingManager { get; }
        IJobSelector JobSelector { get; }
        IJobMediaItemSelector JobMediaItemSelector { get; }
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
    }
}
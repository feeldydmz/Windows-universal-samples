﻿using System.Collections.Generic;
using Megazone.Core.VideoTrack.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.Serializable
{
    internal static class SubtitleTabItemParser
    {
        public static IList<SubtitleTabItem> Convert(IList<ISubtitleTabItemViewModel> tabItemViewModels)
        {
            IList<SubtitleTabItem> tabItems = new List<SubtitleTabItem>();
            foreach (var tab in tabItemViewModels)
            {
                var tabItem = new SubtitleTabItem(tab.Caption)
                {
                    Name = tab.Name,
                    LanguageCode = tab.LanguageCode,
                    CountryCode = tab.CountryCode,
                    Kind = tab.Kind,
                    IsSelected = tab.IsSelected,
                    FilePath = tab.FilePath,
                    VideoId = tab.VideoId,
                    CaptionAssetId = tab.CaptionAssetId
                };

                IList<SubtitleItem> rows = new List<SubtitleItem>();
                foreach (var row in tab.Rows)
                    rows.Add(new SubtitleItem
                    {
                        Number = row.Number,
                        StartTime = row.StartTime,
                        EndTime = row.EndTime,
                        Texts = row.Texts
                    });
                tabItem.Rows = rows;
                tabItems.Add(tabItem);
            }

            return tabItems;
        }
    }
}
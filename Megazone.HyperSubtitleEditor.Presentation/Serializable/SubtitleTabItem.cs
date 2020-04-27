using System;
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.VideoTrack.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.Serializable
{
    [Serializable]
    internal class SubtitleTabItem
    {
        public SubtitleTabItem(Caption caption)
        {
            Caption = caption;
        }

        public string Name { get; set; }
        public CaptionKind Kind { get; set; }
        public string LanguageCode { get; set; }
        public string CountryCode { get; set; }
        public IList<SubtitleItem> Rows { get; set; }

        public bool IsSelected { get; set; }
        public string FilePath { get; set; }

        public Caption Caption { get; }
        public string VideoId { get; set; }
        public string CaptionAssetId { get; set; }
    }
}
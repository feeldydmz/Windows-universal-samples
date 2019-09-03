using System;
using System.Collections.Generic;
using Megazone.Api.Transcoder.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.VideoTrack.Model;

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
        public TrackKind Kind { get; set; }
        public string LanguageCode { get; set; }
        public IList<SubtitleItem> Rows { get; set; }

        public bool IsSelected { get; set; }

        // TODO: MCM에서는 사용하지 않는 모델. 제거대상.
        /// <summary>
        ///     [TODO] Track을 Caption으로 대체한다.
        /// </summary>
        public Track Track { get; set; }

        public string FilePath { get; set; }

        // 기존 Track에 대응됨.
        public Caption Caption { get; }
        public string VideoId { get; set; }
        public string CaptionAssetId { get; set; }
    }
}
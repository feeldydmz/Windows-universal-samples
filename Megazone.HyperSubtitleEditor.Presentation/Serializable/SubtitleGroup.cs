using System;
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Serializable
{
    [Serializable]
    internal class SubtitleGroup
    {
        public SubtitleGroup(IList<SubtitleTabItem> tabs, string videoId, string captionAssetId, Stage stage, Project project)
        {
            Tabs = tabs;
            VideoId = videoId;
            CaptionAssetId = captionAssetId;
            Stage = stage;
            Project = project;
            Stage = stage;
        }
        
        public IList<SubtitleTabItem> Tabs { get; }
        public string VideoId { get; }
        public string CaptionAssetId { get; }
        public Stage Stage { get; }
        public Project Project { get; }
    }
}
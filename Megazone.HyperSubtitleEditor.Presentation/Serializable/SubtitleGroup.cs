using System;
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Serializable
{
    [Serializable]
    internal class SubtitleGroup
    {
        public SubtitleGroup(string profileId, string pipelineId, string jobId, string region,
            IList<SubtitleTabItem> tabs, string videoId, string captionAssetId, Stage stage, Project project)
        {
            ProfileId = profileId;
            PipelineId = pipelineId;
            JobId = jobId;
            Region = region;
            Tabs = tabs;
            VideoId = videoId;
            CaptionAssetId = captionAssetId;
            Stage = stage;
            Project = project;
            Stage = stage;
        }

        public string Region { get; }
        public string ProfileId { get; }
        public string PipelineId { get; }
        public string JobId { get; }
        public IList<SubtitleTabItem> Tabs { get; }
        public string VideoId { get; }
        public string CaptionAssetId { get; }
        public Stage Stage { get; }
        public Project Project { get; }
    }
}
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using System;
using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Presentation.Serializable
{
    [Serializable]
    internal class SubtitleGroup
    {
        public SubtitleGroup(string profileId, string pipelineId, string jobId, string region,
            IList<SubtitleTabItem> tabs, string videoId, string captionAssetId)
        {
            ProfileId = profileId;
            PipelineId = pipelineId;
            JobId = jobId;
            Region = region;
            Tabs = tabs;
            VideoId = videoId;
            CaptionAssetId = captionAssetId;
        }

        public string Region { get; }
        public string ProfileId { get; }

        public string PipelineId { get; }

        public string JobId { get; }
        public IList<SubtitleTabItem> Tabs { get; }
        public string VideoId { get; }
        public string CaptionAssetId { get; }
    }
}
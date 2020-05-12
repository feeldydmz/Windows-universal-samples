using System.Collections.Generic;
using System.Linq;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    public class AppContext
    {
        public static McmData McmOpenData { get; private set; }


        public static void SetMcmOpenData(IEnumerable<KeyValuePair<string, string>> arguments)
        {
            var argumentList = arguments?.ToList() ?? new List<KeyValuePair<string, string>>();
            if (!argumentList.Any())
                return;

            var creator = new McmData.Creator();
            foreach (var argument in argumentList)
                switch (argument.Key)
                {
                    case "authorization":
                        creator.SetAuthorization(argument.Value);
                        break;
                    case "stageId":
                        creator.SetStageId(argument.Value);
                        break;
                    case "projectId":
                        creator.SetProjectId(argument.Value);
                        break;
                    case "videoId":
                        creator.SetVideoId(argument.Value);
                        break;
                    case "assetId":
                        creator.SetAssetId(argument.Value);
                        break;
                    case "captionIds":
                        creator.SetCaptionIds(ConvertToList(argument.Value));
                        break;
                }

            McmOpenData = creator.Create();

            List<string> ConvertToList(string args)
            {
                return string.IsNullOrEmpty(args) ? null : args.Split(',').Select(id => id.Trim()).ToList();
            }
        }

        public struct McmData
        {
            public string Authorization { get; }
            public string StageId { get; }
            public string ProjectId { get; }
            public string VideoId { get; private set; }
            public string AssetId { get; private set; }
            public IEnumerable<string> CaptionIds { get; private set; }

            public McmData(string authorization, 
                string stageId, 
                string projectId)
            {
                Authorization = authorization;
                StageId = stageId;
                ProjectId = projectId;
                VideoId = null;
                AssetId = null;
                CaptionIds = null;
            }

            public class Creator
            {
                private string _authorization;
                private string _assetId;
                private IEnumerable<string> _captionIds;
                private string _projectId;
                private string _stageId;
                private string _videoId;

                public McmData Create()
                {
                    var configuration = new McmData(_authorization, _stageId, _projectId)
                    {
                        VideoId = _videoId, 
                        AssetId = _assetId,
                        CaptionIds = _captionIds
                    };

                    return configuration;
                }
                public Creator SetAuthorization(string authorization)
                {
                    _authorization = authorization;
                    return this;
                }

                public Creator SetStageId(string stageId)
                {
                    _stageId = stageId;
                    return this;
                }

                public Creator SetProjectId(string projectId)
                {
                    _projectId = projectId;
                    return this;
                }

                public Creator SetVideoId(string videoId)
                {
                    _videoId = videoId;
                    return this;
                }

                public Creator SetAssetId(string assetId)
                {
                    _assetId = assetId;
                    return this;
                }

                public Creator SetCaptionIds(IEnumerable<string> captionIds)
                {
                    _captionIds = captionIds?.ToList();
                    return this;
                }
            }
        }
    }
}
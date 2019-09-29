using System;
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Newtonsoft.Json;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Serializable]
    public sealed class RecentlyItem
    {
        private RecentlyItem()
        {
            CreatedTime = DateTime.UtcNow;
        }
        [JsonProperty]
        public DateTime CreatedTime { get; }
        [JsonProperty]
        public string StageId { get; private set; }
        [JsonProperty]
        public string ProjectId { get; private set; }
        [JsonProperty]
        public bool IsOnLine { get; private set; }
        [JsonProperty]
        public Video Video { get; private set; }
        [JsonProperty]
        public CaptionAsset CaptionAsset { get; private set; }
        [JsonProperty]
        public IEnumerable<Caption> Captions { get; private set; }
        [JsonProperty]
        public string LocalFileFullPath { get; private set; }
        [JsonProperty]
        public string Format { get; private set; }

        public class OnlineRecentlyCreator
        {
            private CaptionAsset _captionAsset;
            private IEnumerable<Caption> _captions;
            private Video _video;
            private readonly SignInViewModel _signIn;

            public OnlineRecentlyCreator()
            {
                _signIn = Bootstrapper.Container.Resolve<SignInViewModel>();
            }

            public virtual RecentlyItem Create()
            {
                var recently = new RecentlyItem
                {
                    StageId = _signIn.SelectedStage?.Id,
                    ProjectId = _signIn.SelectedProject?.ProjectId,
                    IsOnLine = true,
                    Video = _video,
                    CaptionAsset = _captionAsset,
                    Captions = _captions
                };
                return recently;
            }


            public OnlineRecentlyCreator SetVideo(Video video)
            {
                _video = video;
                return this;
            }

            public OnlineRecentlyCreator SetCaptionAsset(CaptionAsset captionAsset)
            {
                _captionAsset = captionAsset;
                return this;
            }

            public OnlineRecentlyCreator SetCaptions(IEnumerable<Caption> captions)
            {
                _captions = captions;
                return this;
            }
        }

        public class OfflineRecentlyCreator : OnlineRecentlyCreator
        {
            private string _format;
            private string _localFileFullPath;

            public override RecentlyItem Create()
            {
                var recently = base.Create();
                recently.LocalFileFullPath = _localFileFullPath;
                recently.Format = _format;
                return recently;
            }

            public OfflineRecentlyCreator SetLocalFileFullPath(string localFileFullPath)
            {
                _localFileFullPath = localFileFullPath;
                return this;
            }

            public OfflineRecentlyCreator SetFormat(string format)
            {
                _format = format;
                return this;
            }
        }
    }
}
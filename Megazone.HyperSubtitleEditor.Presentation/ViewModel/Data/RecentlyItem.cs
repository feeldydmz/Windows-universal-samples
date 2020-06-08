using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.VideoTrack;
using Megazone.Core.Windows.Xaml.Behaviors.Primitives;
using Newtonsoft.Json;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Serializable]
    public sealed class RecentlyItem
    {
        private string _firstId;
        private string _secondId;

        [JsonProperty] public DateTime CreatedTime { get; private set; }

        [JsonProperty] public string StageId { get; private set; }

        [JsonProperty] public string ProjectId { get; private set; }

        [JsonProperty] public bool IsOnLine { get; private set; }

        [JsonProperty] public Video Video { get; private set; }

        [JsonProperty] public CaptionAsset CaptionAsset { get; private set; }

        [JsonProperty] public IEnumerable<Caption> Captions { get; private set; }

        [JsonProperty] public string LocalFileFullPath { get; private set; }

        [JsonProperty] public SubtitleFormatKind Format { get; private set; }

        [JsonProperty] public string FirstName { get; private set; }

        [JsonProperty]
        public string FirstId
        {
            get => _firstId;
            private set
            {
                _firstId = value;

                if (_firstId == "0")
                    _firstId = "";
            }
        }

        [JsonProperty] public string SecondName { get; private set; }
        
        public string SecondId
        {
            get => _secondId;
            private set
            {
                _secondId = value;
                if (_secondId == "0")
                    _secondId = "";
            }
        }

        [JsonIgnore]
        public IEnumerable<Caption> LatestCaptions { get; set; }

        public class OnlineRecentlyCreator
        {
            private readonly SignInViewModel _signIn;
            private CaptionAsset _captionAsset;
            private IEnumerable<Caption> _captions;
            private Video _video;

            public OnlineRecentlyCreator()
            {
                _signIn = Bootstrapper.Container.Resolve<SignInViewModel>();
            }

            public virtual RecentlyItem Create()
            {

                var latestCaptions = (_captions != null && _captions.Count() > 2) 
                    ? _captions.ToList().GetRange(0, 2) 
                    : _captions;

                var recently = new RecentlyItem
                {
                    StageId = _signIn.SelectedStage?.Id,
                    ProjectId = _signIn.SelectedProject?.ProjectId,
                    IsOnLine = true,
                    Video = _video,
                    CreatedTime = DateTime.UtcNow,
                    CaptionAsset = _captionAsset,
                    Captions = _captions,
                    LatestCaptions = latestCaptions
                };

                if (recently.Video == null && recently.CaptionAsset != null)
                {
                    recently.FirstName = recently.CaptionAsset.Name;
                    recently.FirstId = $"({recently.CaptionAsset.Id})";

                    recently.SecondName = "";
                    recently.SecondId = "";
                }
                else if (recently.Video != null && recently.CaptionAsset == null)
                {
                    recently.FirstName = recently.Video.Name;
                    recently.FirstId = $"({recently.Video.Id})";

                    recently.SecondName = "";
                    recently.SecondId = "";
                }
                else if (recently.Video != null && recently.CaptionAsset != null)
                {
                    recently.FirstName = recently.Video.Name;
                    recently.FirstId = $"({recently.Video.Id})";

                    recently.SecondName = recently.CaptionAsset.Name;
                    recently.SecondId = $"({recently.CaptionAsset.Id})";
                }

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
            private SubtitleFormatKind _format;
            private string _localFileFullPath;

            public override RecentlyItem Create()
            {
                var recently = base.Create();
                recently.IsOnLine = false;
                recently.LocalFileFullPath = _localFileFullPath;
                recently.Format = _format;

                recently.FirstName = Path.GetFileName(recently.LocalFileFullPath);
                recently.SecondName = recently.LocalFileFullPath;

                return recently;
            }

            public OfflineRecentlyCreator SetLocalFileFullPath(string localFileFullPath)
            {
                _localFileFullPath = localFileFullPath;
                return this;
            }

            public OfflineRecentlyCreator SetFormat(SubtitleFormatKind format)
            {
                _format = format;
                return this;
            }
        }
    }
}
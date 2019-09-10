using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Api.Transcoder.Domain;
using Megazone.Api.Transcoder.ServiceInterface;
using Megazone.Cloud.Aws.Domain;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    public class AppContext
    {
        internal static Configuration Config { get; private set; }
        internal static Job Job { get; private set; }
        internal static CredentialInfo CredentialInfo { get; private set; }
        public static McmData McmOpenData { get; private set; }

        public void SetConfig(string profileId, string pipelineId, string jobId, string region)
        {
            Config = new Configuration(profileId, pipelineId, jobId, region);
        }

        public void Initialize(IJobService jobService, Action<bool> completeAction)
        {
            var profiles = ClientProfileManager.Instance.Load();
            var profile = profiles?.Profiles?.FirstOrDefault(p => p.Id == Config.ProfileId);

            if (profile == null)
            {
#if DEBUG
                profile = profiles?.Profiles?.FirstOrDefault();
                if (profile == null)
                {
                    completeAction?.Invoke(true);
                    return;
                }
#else
                completeAction?.Invoke(true);
                return;
#endif
            }

            if (string.IsNullOrEmpty(Config.PipelineId))
            {
                completeAction?.Invoke(true);
                return;
            }

            var profileRegion = profiles.ProfileRegions?.FirstOrDefault(p => p.ProfileID == profile.Id);
            RegionManager.Instance.Initialize(profileRegion?.RegionInformations);
            var matchingRegion = RegionManager.Instance.Regions.FirstOrDefault(r => r.Code == Config.Region);
            if (matchingRegion != null)
                RegionManager.Instance.Current = matchingRegion;

            CredentialInfo = new CredentialInfo(profile.ClientProfileCredentialInfo.Credential.ServiceType,
                profile.ClientProfileCredentialInfo.Credential.AccessKeyId,
                profile.ClientProfileCredentialInfo.Credential.SecretAccessKey, RegionManager.Instance.Current.Code);

            PipelineLoader.Instance.LoadPipeline(Config.PipelineId, CredentialInfo,
                success =>
                {
                    if (!success)
                    {
                        completeAction?.Invoke(false);
                        return;
                    }

                    if (string.IsNullOrEmpty(Config.JobId))
                    {
                        completeAction?.Invoke(true);
                        return;
                    }

                    try
                    {
                        Job = jobService.Get(RegionManager.Instance.Current.API, Config.JobId,
                            PipelineLoader.Instance.SelectedPipeline.Notifications.Completed);
                        if (Job != null)
                            this.InvokeOnUi(() => { MessageCenter.Instance.Send(new JobFoundMessage(this, Job)); });
                    }
                    finally
                    {
                        completeAction?.Invoke(Job != null);
                    }
                });
        }

        internal static void SetJob(Job job)
        {
            Job = job;
        }

        public static void SetMcmOpenData(IEnumerable<KeyValuePair<string, string>> arguments)
        {
            var argumentList = arguments?.ToList() ?? new List<KeyValuePair<string, string>>();
            if (!argumentList.Any())
                return;

            var creator = new McmData.Creator();
            foreach (var argument in argumentList)
                switch (argument.Key)
                {
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

        public struct Configuration
        {
            public Configuration(string profileId, string pipelineId, string jobId, string region)
            {
                Region = region;
                ProfileId = profileId;
                PipelineId = pipelineId;
                JobId = jobId;
            }

            public string Region { get; }
            public string PipelineId { get; }

            public string JobId { get; }

            public string ProfileId { get; }
        }


        public struct McmData
        {
            public string StageId { get; }
            public string ProjectId { get; }
            public string VideoId { get; private set; }
            public string AssetId { get; private set; }

            public McmData(string stageId, string projectId)
            {
                StageId = stageId;
                ProjectId = projectId;
                VideoId = null;
                AssetId = null;
            }

            public class Creator
            {
                private string _assetId;
                private string _projectId;
                private string _stageId;
                private string _videoId;
                private IEnumerable<string> _captionIds;

                public McmData Create()
                {
                    var configuration = new McmData(_stageId, _projectId)
                        {VideoId = _videoId, AssetId = _assetId};

                    return configuration;
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
                    _captionIds = _captionIds?.ToList();
                    return this;
                }
            }
        }
    }
}
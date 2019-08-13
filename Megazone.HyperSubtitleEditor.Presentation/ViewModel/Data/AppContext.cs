using System;
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
    }
}
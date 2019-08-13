using System;
using System.Linq;
using Megazone.Cloud.Aws.Domain;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Model;
using Megazone.Cloud.Transcoder.Repository.ElasticTranscoder;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile;
using Unity;

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

        public void Initialize(ITranscodingRepository transcodingRepository, Action<bool> completeAction)
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
            var matchingRegion = RegionManager.Instance.Regions.FirstOrDefault(r => r.Name == Config.Region);
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
                    var parameter = new ParameterBuilder(CredentialInfo).Build();
                    try
                    {
                        Job = transcodingRepository.FindJob(parameter, Config.JobId) as Job;
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
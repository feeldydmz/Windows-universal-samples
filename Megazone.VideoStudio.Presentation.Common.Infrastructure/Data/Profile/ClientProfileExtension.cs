using Megazone.Cloud.Common.Domain;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    internal static class ClientProfileExtension
    {
        public static ClientProfile Clone(this ClientProfile clientProfile)
        {
            return new ClientProfile(clientProfile.Id)
            {
                Name = clientProfile.Name,
                ClientProfileCredentialInfo = clientProfile.ClientProfileCredentialInfo,
                RememberedPipelineIdForAutoConnection = clientProfile.RememberedPipelineIdForAutoConnection
            };
        }

        public static ClientProfileModel ToPersistentModel(this ClientProfile clientProfile, bool shouldEncrypt)
        {
            return new ClientProfileModel(clientProfile.Id,
                clientProfile.Name,
                new ClientProfileCredentialInfoModel(ServiceType.AwsElasticTranscoder,
                    clientProfile.ClientProfileCredentialInfo.Credential.GetJson(shouldEncrypt)),
                clientProfile.LastAccessedDateTimeTicks,
                clientProfile.RememberedPipelineIdForAutoConnection);
        }
    }
}
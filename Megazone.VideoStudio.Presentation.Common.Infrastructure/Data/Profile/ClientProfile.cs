using Megazone.Cloud.Common.Domain;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    public class ClientProfile
    {
        public ClientProfile(string id)
        {
            Id = id;
        }

        public ClientProfile(ClientProfileModel data)
        {
            Id = data.Id;
            Name = data.Name;
            ClientProfileCredentialInfo =
                new ClientProfileCredentialInfo(data.CredentialInfo.CredentialJson, ServiceType.AwsElasticTranscoder);
            LastAccessedDateTimeTicks = data.LastAccessedDateTimeTicks;
            RememberedPipelineIdForAutoConnection = data.RememberedPipelineIdForAutoConnection;
        }

        public string Id { get; }
        public string Name { get; set; }
        public ClientProfileCredentialInfo ClientProfileCredentialInfo { get; set; }
        public long LastAccessedDateTimeTicks { get; set; }

        public string RememberedPipelineIdForAutoConnection { get; set; }

        public ClientProfile Clone()
        {
            var clientProfile = new ClientProfile(Id)
            {
                Name = Name,
                ClientProfileCredentialInfo = ClientProfileCredentialInfo,
                RememberedPipelineIdForAutoConnection = RememberedPipelineIdForAutoConnection
            };
            return clientProfile;
        }
    }
}
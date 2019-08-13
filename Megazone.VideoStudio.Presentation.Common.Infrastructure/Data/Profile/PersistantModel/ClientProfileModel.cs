using Newtonsoft.Json;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel
{
    public class ClientProfileModel
    {
        public ClientProfileModel(string id,
            string name,
            ClientProfileCredentialInfoModel credentialInfo,
            long lastAccessedDateTimeTicks,
            string rememberedPipelineIdForAutoConnection)
        {
            Id = id;
            Name = name;
            CredentialInfo = credentialInfo;
            LastAccessedDateTimeTicks = lastAccessedDateTimeTicks;
            RememberedPipelineIdForAutoConnection = rememberedPipelineIdForAutoConnection;
        }

        public string Id { get; }
        public string Name { get; }

        [JsonProperty("ClientProfileCredentialInfo")]
        public ClientProfileCredentialInfoModel CredentialInfo { get; }

        public long LastAccessedDateTimeTicks { get; }
        public string RememberedPipelineIdForAutoConnection { get; set; }
    }
}
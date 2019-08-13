using Megazone.Cloud.Common.Domain;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel
{
    public class ClientProfileCredentialInfoModel
    {
        public ClientProfileCredentialInfoModel(ServiceType serviceType, string credentialJson)
        {
            ServiceType = serviceType;
            CredentialJson = credentialJson;
        }

        public ServiceType ServiceType { get; }
        public string CredentialJson { get; }
    }
}
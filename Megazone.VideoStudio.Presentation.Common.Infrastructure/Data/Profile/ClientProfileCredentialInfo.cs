using Megazone.Cloud.Aws.Domain;
using Megazone.Cloud.Common.Domain;
using Megazone.Cloud.Transcoder.Repository.ElasticTranscoder;
using Newtonsoft.Json;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    public class ClientProfileCredentialInfo
    {
        private CredentialInfo _credential;

        public ClientProfileCredentialInfo(string credentialJson, ServiceType serviceType)
        {
            CredentialJson = credentialJson;
            ServiceType = serviceType;
        }

        [JsonIgnore]
        public CredentialInfo Credential
        {
            get
            {
                if (_credential != null)
                    return _credential;

                _credential = (CredentialInfo) new ParameterProvider().MakeCredentialInfo(CredentialJson);

                return _credential;
            }
        }

        public ServiceType ServiceType { get; }

        // 마지막 선택 Region을 저장해야 해서 set 추가
        public string CredentialJson { get; set; }

        public ClientProfileCredentialInfo Clone()
        {
            return new ClientProfileCredentialInfo(CredentialJson, ServiceType);
        }
    }
}
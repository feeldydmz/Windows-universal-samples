namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel
{
    internal class CdnProfileItemModel
    {
        public CdnProfileItemModel(string id, string name, ClientProfileCredentialInfoModel credentialInfo)
        {
            Name = name;
            CredentialInfo = credentialInfo;
            Id = id;
        }

        public ClientProfileCredentialInfoModel CredentialInfo { get; }
        public string Name { get; }
        public string Id { get; }
    }
}
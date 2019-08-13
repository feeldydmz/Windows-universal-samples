namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    public class EncryptedClientProfiles
    {
        public EncryptedClientProfiles(string encryptedJson)
        {
            EncryptedJson = encryptedJson;
        }

        public string EncryptedJson { get; }
    }
}
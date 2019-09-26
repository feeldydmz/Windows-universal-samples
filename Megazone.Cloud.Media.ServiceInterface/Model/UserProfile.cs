using Megazone.Cloud.Media.Domain;

namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class UserProfile
    {
        public UserProfile(MeResponse response)
        {
            Name = response.Megaone.Name;
            Username = response.Megaone.Username;
            Type = response.Type;
        }

        public string Name { get; }
        public string Username { get; }
        public string Type { get; }
    }
}
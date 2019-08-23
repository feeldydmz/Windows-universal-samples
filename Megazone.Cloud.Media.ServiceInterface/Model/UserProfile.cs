using System.Collections.Generic;
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
            //LatestAccessedAt = string.IsNullOrEmpty(response.LatestAccessedAt) ? 
            //    DateTime.MinValue : 
            //    DateTimeOffset.Parse(response.LatestAccessedAt).Date;
            Stages = response.Stages;
        }

        public string Name { get; }
        public string Username { get; }

        public string Type { get; }

        //public DateTime LatestAccessedAt { get; set; }
        public IEnumerable<Stage> Stages { get; }
    }
}
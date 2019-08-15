using System;
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
            LatestAccessedAt = DateTimeOffset.Parse(response.LatestAccessedAt);
            Stages = response.Stages;
        }

        public string Name { get; }
        public string Username { get; }
        public string Type { get; }
        public DateTimeOffset LatestAccessedAt { get; set; }
        public IEnumerable<Stage> Stages { get; }
    }
}
using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class MeResponse
    {
        public MeResponse(Megaone megaone, string type, string latestAccessedAt, IEnumerable<Stage> stages)
        {
            Megaone = megaone;
            Type = type;
            LatestAccessedAt = latestAccessedAt;
            Stages = stages;
        }

        public Megaone Megaone { get; }
        public string Type { get; }
        public string LatestAccessedAt { get; }
        public IEnumerable<Stage> Stages { get; }
    }
}
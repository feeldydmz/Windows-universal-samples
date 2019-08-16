using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class Video
    {
        public Video(string id, string name, string description, string status, long duration, string createdAt,
            IEnumerable<Asset> origins, IEnumerable<Asset> sources, IEnumerable<Asset> captions,
            IEnumerable<Asset> thumbnails, IEnumerable<Asset> posters)
        {
            Id = id;
            Name = name;
            Description = description;
            Status = status;
            Duration = duration;
            CreatedAt = createdAt;

            Origins = origins;
            Sources = sources;
            Captions = captions;
            Thumbnails = thumbnails;
            Posters = posters;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Status { get; }
        public long Duration { get; }
        public string CreatedAt { get; }
        public IEnumerable<Asset> Origins { get; }
        public IEnumerable<Asset> Sources { get; }
        public IEnumerable<Asset> Captions { get; }
        public IEnumerable<Asset> Thumbnails { get; }
        public IEnumerable<Asset> Posters { get; }
    }
}
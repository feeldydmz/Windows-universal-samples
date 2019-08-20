using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class Video
    {
        public Video(string id, string name, string description, string status, long duration, string createdAt,
            IEnumerable<RenditionAsset> origins, IEnumerable<RenditionAsset> sources, IEnumerable<CaptionAsset> captions,
            IEnumerable<ThumbnailAsset> thumbnails, IEnumerable<Poster> posters)
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
        public IEnumerable<RenditionAsset> Origins { get; }
        public IEnumerable<RenditionAsset> Sources { get; }
        public IEnumerable<CaptionAsset> Captions { get; }
        public IEnumerable<ThumbnailAsset> Thumbnails { get; }
        public IEnumerable<Poster> Posters { get; }
    }
}
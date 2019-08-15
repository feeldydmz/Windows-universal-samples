using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class Video
    {
        public Video(string id, string name, IEnumerable<Asset> origins, IEnumerable<Asset> sources,
            IEnumerable<Asset> captions, IEnumerable<Asset> thumbnails, IEnumerable<Asset> posters)
        {
            Id = id;
            Name = name;
            Origins = origins;
            Sources = sources;
            Captions = captions;
            Thumbnails = thumbnails;
            Posters = posters;
        }

        public string Id { get; }
        public string Name { get; }
        public IEnumerable<Asset> Origins { get; }
        public IEnumerable<Asset> Sources { get; }
        public IEnumerable<Asset> Captions { get; }
        public IEnumerable<Asset> Thumbnails { get; }
        public IEnumerable<Asset> Posters { get; }
    }
}
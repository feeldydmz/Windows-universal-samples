using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class RenditionAsset : Asset<Rendition>
    {
        public RenditionAsset(string id, string name, string status, string type, string mediaType, long duration,
            int version, string createdAt, IEnumerable<string> urls, IEnumerable<Rendition> elements)
            : base(id, name, status, type, mediaType, duration, version, createdAt, elements)
        {
            Urls = urls;
        }

        public IEnumerable<string> Urls { get; }
    }
}
using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class RenditionAsset : Asset<Rendition>
    {
        public RenditionAsset(string id, string name, string status, string type, string mediaType, long duration,
            int version, string createdAt, object urls, IEnumerable<Rendition> elements)
            : base(id, name, status, type, mediaType, duration, version, createdAt, elements)
        {
            if (urls is string url)
            {
                Urls = new List<string>() { url };
            }
            else if (urls is IEnumerable<string> list)
            {
                Urls = list;
            }
        }

        public IEnumerable<string> Urls { get; }
    }
}
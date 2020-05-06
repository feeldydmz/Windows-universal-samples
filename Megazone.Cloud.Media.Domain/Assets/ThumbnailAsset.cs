using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class ThumbnailAsset : Asset<Thumbnail>
    {
        public ThumbnailAsset(string id, string name, string status, string type, string mediaType, string ingestType,
            long duration, int version, string createdAt, IEnumerable<Thumbnail> elements, object urls, string folderPath) : base(id, name,
            status, type, mediaType, ingestType, duration, version, createdAt, elements, null, folderPath)
        {
            if (urls is string url)
                Urls = new List<string> {url};
            else if (urls is IEnumerable<string> list) Urls = list;
        }

        public IEnumerable<string> Urls { get; }
    }
}
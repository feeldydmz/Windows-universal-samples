using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class CaptionAsset : Asset<Caption>
    {
        public CaptionAsset(string id, string name, string status, string type, string mediaType, long duration, int version, string createdAt, IEnumerable<Caption> elements)
            : base(id, name, status, type, mediaType, duration, version, createdAt, elements)
        {
        }
    }
}

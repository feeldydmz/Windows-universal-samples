using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public interface IAsset
    {
        string Id { get; }
        string Name { get; }
        string Status { get; }
        string Type { get; }
        string MediaType { get; }
        long Duration { get; }
        int Version { get; }
        string CreatedAt { get; }
    }

    public interface IAsset<TElement> : IAsset where TElement : IAssetElement
    {
        IEnumerable<TElement> Elements { get; }
    }
}
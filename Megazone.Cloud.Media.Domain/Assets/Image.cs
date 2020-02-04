namespace Megazone.Cloud.Media.Domain.Assets
{
    public class Image : IAssetElement
    {
        public Image(string id, bool isPreferred, string url, string accessUrl, long size, int height, int width)
        {
            Id = id;
            IsPreferred = isPreferred;
            Url = url;
            AccessUrl = accessUrl;
            Size = size;
            Height = height;
            Width = width;
        }

        public string Id { get; }
        public bool IsPreferred { get; }
        public string Url { get; }
        public string AccessUrl { get; }
        public long Size { get; }
        public int Height { get; }
        public int Width { get; }
    }

    public class Thumbnail : Image
    {
        public Thumbnail(string id, bool isPreferred, string url, string accessUrl, long size, int height, int width, int time) : base(id,
            isPreferred, url, accessUrl, size, height, width)
        {
            Time = time;
        }

        public int Time { get; }
    }

    public class Poster : Image
    {
        public Poster(string assetId, string id, bool isPreferred, string url, string accessUrl, long size, int height, int width) : base(
            id, isPreferred, url, accessUrl, size, height, width)
        {
            AssetId = assetId;
        }

        public string AssetId { get; }
    }
}
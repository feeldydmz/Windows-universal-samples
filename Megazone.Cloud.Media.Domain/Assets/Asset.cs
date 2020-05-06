using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public abstract class Asset<TAssetElement> : IAsset<TAssetElement> where TAssetElement : IAssetElement
    {
        protected Asset(string id, string name, string status, string type, string mediaType, string ingestType,
            long duration, int version, string createdAt, IEnumerable<TAssetElement> elements, IEnumerable<string> encryptions, string folderPath)
        {
            Id = id;
            Name = name;
            Status = status;
            Type = type;
            MediaType = mediaType;
            IngestType = ingestType;
            Duration = duration;
            Version = version;
            CreatedAt = createdAt;
            Elements = elements;
            Encryptions = encryptions;
            FolderPath = folderPath;
        }

        public string IngestType { get; }
        public string Id { get; }
        public string Name { get; }
        public string Status { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public IEnumerable<string> Encryptions { get; }
        public string CreatedAt { get; }
        public IEnumerable<TAssetElement> Elements { get; set; }
        public string FolderPath { get; }
    }
}
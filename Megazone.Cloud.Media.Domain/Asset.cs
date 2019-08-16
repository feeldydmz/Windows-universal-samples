namespace Megazone.Cloud.Media.Domain
{
    public class Asset
    {
        public Asset(string id, string name, string status, string type, string mediaType, long duration, int version,
            string createdAt)
        {
            Id = id;
            Name = name;
            Status = status;
            Type = type;
            MediaType = mediaType;
            Duration = duration;
            Version = version;
            CreatedAt = createdAt;
        }

        public string Id { get; }
        public string Name { get; }
        public string Status { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public string CreatedAt { get; }
    }
}
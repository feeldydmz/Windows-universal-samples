using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public interface IAsset<TElement> where TElement : IAssetElement
    {
        string Id { get; }
        string Name { get; }
        string Status { get; }
        string Type { get; }
        string MediaType { get; }
        long Duration { get; }
        int Version { get; }
        string CreatedAt { get; }
        IEnumerable<TElement> Elements { get; }
    }

    public class Asset<TAssetElement> : IAsset<TAssetElement> where TAssetElement : IAssetElement
    {
        public Asset(string id, string name, string status, string type, string mediaType, long duration, int version, 
            string createdAt, IEnumerable<TAssetElement> elements)
        {
            Id = id;
            Name = name;
            Status = status;
            Type = type;
            MediaType = mediaType;
            Duration = duration;
            Version = version;
            CreatedAt = createdAt;
            Elements = elements;
        }

        public string Id { get; }
        public string Name { get; }
        public string Status { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public string CreatedAt { get; }
        public IEnumerable<TAssetElement> Elements { get; }
    }

    public class CaptionAsset : Asset<Caption>
    {
        public CaptionAsset(string id, string name, string status, string type, string mediaType, long duration, int version, string createdAt, IEnumerable<Caption> elements) : base(id, name, status, type, mediaType, duration, version, createdAt, elements)
        {
        }
    }

    public class ThumbnailAsset : Asset<Thumbnail>
    {
        public ThumbnailAsset(string id, string name, string status, string type, string mediaType, long duration, int version, string createdAt, IEnumerable<Thumbnail> elements) : base(id, name, status, type, mediaType, duration, version, createdAt, elements)
        {
        }
    }

    public class RenditionAsset : Asset<Rendition>
    {
        public RenditionAsset(string id, string name, string status, string type, string mediaType, long duration, int version, string createdAt, IEnumerable<Rendition> elements) : base(id, name, status, type, mediaType, duration, version, createdAt, elements)
        {
        }
    }

    public interface IAssetElement
    {
        
    }

    public class Rendition : IAssetElement
    {
        public Rendition(IEnumerable<AudioSetting> audioSettings, VideoSetting videoSetting, IEnumerable<string> urls, long duration, long size)
        {
            AudioSettings = audioSettings;
            VideoSetting = videoSetting;
            Urls = urls;
            Duration = duration;
            Size = size;
        }

        public IEnumerable<AudioSetting> AudioSettings { get; }
        public VideoSetting VideoSetting { get; }
        public IEnumerable<string> Urls { get; }
        public long Duration { get; }
        public long Size { get; }
    }

    public class VideoSetting
    {
        public VideoSetting(long bitrate,string codec, int width, int height, string ratingControlMode)
        {
            Bitrate = bitrate;
            Codec = codec;
            Width = width;
            Height = height;
            RatingControlMode = ratingControlMode;
        }

        public long Bitrate { get; }
        public string Codec { get; }
        public int Width { get; }
        public int Height { get; }
        public string RatingControlMode { get; }
    }

    public class AudioSetting
    {
        public AudioSetting(long bitrate, string codec, long sampleRate, string ratingControlMode)
        {
            Bitrate = bitrate;
            Codec = codec;
            SampleRate = sampleRate;
            RatingControlMode = ratingControlMode;
        }

        public long Bitrate { get; }
        public string Codec { get; }
        public long SampleRate { get; }
        public string RatingControlMode { get; }
    }

    public class Image : IAssetElement
    {
        public Image(string id, bool isPreferred, string url, long size, int height, int width)
        {
            Id = id;
            IsPreferred = isPreferred;
            Url = url;
            Size = size;
            Height = height;
            Width = width;
        }

        public string Id { get; }
        public bool IsPreferred { get; }
        public string Url { get; }
        public long Size { get; }
        public int Height { get; }
        public int Width { get; }
    }

    public class Thumbnail : Image
    {
        public Thumbnail(string id, bool isPreferred, string url, long size, int height, int width, int time) : base(id, isPreferred, url, size, height, width)
        {
            Time = time;
        }

        public int Time { get; }
    }

    public class Poster : Image
    {
        public Poster(string assetId, string id, bool isPreferred, string url, long size, int height, int width) : base(id, isPreferred, url, size, height, width)
        {
            AssetId = assetId;
        }
        public string AssetId { get; }
    }

    public class Caption : IAssetElement
    {
        public Caption(string id, bool isDraft, bool isPreferred, string language, string country, string kind, string label, string url)
        {
            Id = id;
            IsDraft = isDraft;
            IsPreferred = isPreferred;
            Language = language;
            Country = country;
            Kind = kind;
            Label = label;
            Url = url;
        }

        public string Id { get; }
        public bool IsDraft { get; }
        public bool IsPreferred { get; }
        public string Language { get; }
        public string Country { get; }
        public string Kind { get; }
        public string Label { get; }
        public string Url { get; }
    }
}
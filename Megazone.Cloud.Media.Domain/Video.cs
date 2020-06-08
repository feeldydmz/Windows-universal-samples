using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.Cloud.Media.Domain
{
    public class JobMe
    {
        public string Id { get; set; }
    }

    public class Video
    {
        public Video(string id, string name, string description, string status, long duration, string createdAt,
            int version, string assetsFirstImageUrl, Poster primaryPoster, IEnumerable<RenditionAsset> origins, IEnumerable<RenditionAsset> sources,
            IEnumerable<CaptionAsset> captions, IEnumerable<ThumbnailAsset> thumbnails, IEnumerable<Poster> posters, IEnumerable<string> encryptions,
            string folderPath, bool hasCaption = false, JobMe job = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Duration = duration;
            Status = status;
            CreatedAt = createdAt;
            Version = version;
            ImageUrl = assetsFirstImageUrl;
            PrimaryPoster = primaryPoster;
            Origins = origins;
            Sources = sources;
            Captions = captions;
            Thumbnails = thumbnails;
            Posters = posters;
            Encryptions = encryptions;
            HasCaption = hasCaption;
            Job = job;
            FolderPath = folderPath;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Status { get; }
        public long Duration { get; }
        public int Version { get; }
        public string ImageUrl { get; }
        public Poster PrimaryPoster { get; }
        public string CreatedAt { get; }
        public IEnumerable<RenditionAsset> Origins { get; }
        public IEnumerable<RenditionAsset> Sources { get; }
        public IEnumerable<CaptionAsset> Captions { get; }
        public IEnumerable<ThumbnailAsset> Thumbnails { get; }
        public IEnumerable<Poster> Posters { get; }
        public IEnumerable<string> Encryptions { get; }

        public JobMe Job { get; set; }
        public bool HasCaption { get; set; }
        public string FolderPath { get; set; }
    }
}
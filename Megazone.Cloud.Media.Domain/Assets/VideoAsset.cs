﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class ThumnailMe
    {
        public string url { get; set; }
    }

    public class VideoAsset : Asset<Rendition>
    {
        public VideoAsset(string id, string name, string status, string type, string mediaType, string ingestType,
            long duration, int version, string createdAt, IEnumerable<Rendition> elements, IEnumerable<Thumbnail> thumbnails) 
            : base(id, name, status, type,
                mediaType, ingestType, duration, version, createdAt, elements, null)
        {
            Thumbnails = thumbnails;
        }

        //public IEnumerable<ThumnailMe> Thumbnails { get;set }

        public IEnumerable<Thumbnail> Thumbnails
        {
            get;
        }

    }
}
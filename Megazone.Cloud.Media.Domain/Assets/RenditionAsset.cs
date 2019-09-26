﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Megazone.Cloud.Media.Domain.Assets
{
    public class RenditionAsset : Asset<Rendition>
    {
        public RenditionAsset(string id, string name, string status, string type, string mediaType, string ingestType,
            long duration, int version, string createdAt, object urls, IEnumerable<Rendition> elements, IEnumerable<string> encryptions) : base(id, name,
            status, type, mediaType, ingestType, duration, version, createdAt, elements, encryptions)
        {
            if (urls is string url)
                Urls = new List<string> {url};
            else if (urls is IEnumerable<string> list)
                Urls = list;
            else if (urls is JArray jarray) Urls = jarray.ToObject<IEnumerable<string>>();
        }

        public IEnumerable<string> Urls { get; }
    }
}
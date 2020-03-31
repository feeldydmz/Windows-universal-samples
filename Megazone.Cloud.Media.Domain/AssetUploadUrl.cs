using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class AssetUploadUrl
    {
        public AssetUploadUrl(string url, string uploadUrl)
        {
            Url = url;
            UploadUrl = uploadUrl;
        }

        public string Url { get; }
        public string UploadUrl { get;  }
    }
}

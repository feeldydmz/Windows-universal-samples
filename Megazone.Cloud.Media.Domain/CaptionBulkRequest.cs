using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.Cloud.Media.Domain
{
    public class CaptionBulkRequest
    {
        public CaptionBulkRequest(string endpoint, string accessToken, string stageId, string projectId, string assetId,
            int version, IEnumerable<Caption> captions)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            AssetId = assetId;
            Version = version;
            Captions = captions;
        }
        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string AssetId { get; }
        public int Version { get; }
        public IEnumerable<Caption> Captions { get; }
    }
}
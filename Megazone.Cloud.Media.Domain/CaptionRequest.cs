using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.Cloud.Media.Domain
{
    /*
  "id": 381,
  "size": 280,
  "assetId": 8671,
  "language": "fr",
  "country": "FR",
  "kind": "SUB_TITLE",
  "label": "불어",
  "url": "https://mz-cm-transcoding-output.s3.amazonaws.com/mz-cm-v1/test.vtt",
  "isDraft": false,
  "isPreferred": false,
  "mimeType": null
     */
    public class CaptionRequest
    {
        public CaptionRequest(string endpoint, string accessToken, string stageId, string projectId, string assetId, int version,Caption caption)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            AssetId = assetId;
            Version = version;
            Caption = caption;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string AssetId { get; }
        public int Version { get; }
        public Caption Caption { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class UploadCaptionRequest
    {
        public UploadCaptionRequest(string endpoint, string accessToken, string stageId, string projectId,string uploadTargetPath, string text)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            UploadTargetPath = uploadTargetPath;
            Text = text;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string UploadTargetPath { get; }
        public string Text { get; }
    }
}

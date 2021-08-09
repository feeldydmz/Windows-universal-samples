﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class GetUploadUrlRequest
    {
        public GetUploadUrlRequest(string endpoint, string accessToken, string stageId, string projectId, string assetId, string elementId, string fileName, bool shouldOverwrite)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            AssetId = assetId;
            ElementId = elementId;
            FileName = fileName;
            //IsAttachId = isAttachId;
            ShouldOverwrite = shouldOverwrite;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string AssetId { get; }
        public string ElementId { get; }
        public string FileName { get; }
        public bool IsAttachId { get; }
        public bool ShouldOverwrite { get; }
    }
}

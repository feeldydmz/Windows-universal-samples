using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetUploadUrlParameter : RequiredParameter
    {
        public GetUploadUrlParameter(Authorization authorization, string stageId, string projectId,
            string assetId, string fileName, bool shouldOverwrite, string elementId) : base(authorization, stageId, projectId)
        {
            AssetId = assetId;
            FileName = fileName;
            ShouldOverwrite = shouldOverwrite;
            ElementId = elementId;
        }

        public string AssetId { get; }
        public string FileName { get; }
        public bool ShouldOverwrite { get; }
        public string ElementId { get;  }
    }
}

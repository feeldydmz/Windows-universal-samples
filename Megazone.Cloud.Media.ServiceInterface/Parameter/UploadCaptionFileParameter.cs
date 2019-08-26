using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UploadCaptionFileParameter : RequiredParameter
    {
        public UploadCaptionFileParameter(Authorization authorization, string stageId, string projectId,
            string uploadFullPath, string uploadData) : base(authorization, stageId, projectId)
        {
            UploadFullPath = uploadFullPath;
            UploadData = uploadData;
        }

        public string UploadFullPath { get; }
        public string UploadData { get; }
    }
}
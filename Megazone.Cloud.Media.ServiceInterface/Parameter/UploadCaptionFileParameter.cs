using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UploadCaptionFileParameter : RequiredParameter
    {
        public string UploadData { get; }
        public string FileName { get; }
        public string InputPath { get; }
        public string FullPath { get; }

        public UploadCaptionFileParameter(Authorization authorization, string stageId, string projectId,
            string uploadData, string fileName, string inputPath, string fullPath) : base(authorization, stageId,
            projectId)
        {
            UploadData = uploadData;
            FileName = fileName;
            InputPath = inputPath;
            FullPath = fullPath;
        }
    }
}
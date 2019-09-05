namespace Megazone.Cloud.Media.Domain
{
    public class UploadResult
    {
        public UploadResult(string uploadedPath)
        {
            UploadedPath = uploadedPath;
        }

        public string UploadedPath { get; }
    }
}
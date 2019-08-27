namespace Megazone.Cloud.Media.Domain
{
    public class UploadCaptionRequest
    {
        public UploadCaptionRequest(string endpoint, string accessToken, string stageId, string projectId,
            string inputPath, string fileName, string text)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            InputPath = inputPath;
            FileName = fileName;
            Text = text;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string InputPath { get; }
        public string FileName { get; }
        public string Text { get; }
    }
}
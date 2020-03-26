namespace Megazone.Cloud.Media.Domain
{
    public class UploadCaptionRequest
    {
        public UploadCaptionRequest(string uploadUrl, string text)
        {
            UploadUrl = uploadUrl;
            Text = text;
        }

        public string UploadUrl { get; }
        public string Text { get; }
    }
}
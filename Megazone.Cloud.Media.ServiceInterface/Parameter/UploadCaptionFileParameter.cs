using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UploadCaptionFileParameter 
    {
        public UploadCaptionFileParameter(string uploadUrl, string uploadData) 
        {
            UploadData = uploadData;
            //FileName = fileName;
            //InputPath = inputPath;
            UploadUrl = uploadUrl;
        }

        public string UploadData { get; }
        //public string FileName { get; }
        //public string InputPath { get; }
        public string UploadUrl { get; }
    }
}
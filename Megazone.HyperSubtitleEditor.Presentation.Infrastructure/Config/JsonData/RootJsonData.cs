namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData
{
    internal class RootJsonData
    {
        public string ProgramKey { get; set; }

        public GeneralJsonData General { get; set; } = new GeneralJsonData();
        public SubtitleJsonData Subtitle { get; set; } = new SubtitleJsonData();
        public DownloadJsonData Download { get; set; } = new DownloadJsonData();
        public UploadJsonData Upload { get; set; } = new UploadJsonData();
    }
}
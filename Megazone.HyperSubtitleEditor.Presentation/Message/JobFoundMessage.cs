using Megazone.Api.Transcoder.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal class JobFoundMessage : MessageBase
    {
        public JobFoundMessage(object sender, Job job, int selectedOutputIndex = 0,
            bool shouldImportAllSubtitles = true) : base(sender)
        {
            Job = job;
            SelectedOutputIndex = selectedOutputIndex;
            ShouldImportAllSubtitles = shouldImportAllSubtitles;
        }

        public int SelectedOutputIndex { get; }
        public bool ShouldImportAllSubtitles { get; }

        public Job Job { get; }
    }
}
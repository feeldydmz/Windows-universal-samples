using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal class ReinitializeAppContextMessage : MessageBase
    {
        public ReinitializeAppContextMessage(object sender, string pipelineId, string jobId, string profileId,
            string region)
            : base(sender)
        {
            PipelineId = pipelineId;
            JobId = jobId;
            ProfileId = profileId;
            Region = region;
        }

        public string PipelineId { get; }
        public string JobId { get; }
        public string ProfileId { get; }
        public string Region { get; }
    }
}
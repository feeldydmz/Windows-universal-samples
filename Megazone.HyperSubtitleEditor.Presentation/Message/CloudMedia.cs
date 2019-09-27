using DocumentFormat.OpenXml.Presentation;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class CloudMedia
    {
        internal class CaptionOpenMessage : MessageBase
        {
            public CaptionOpenMessage(object sender, CaptionOpenMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public CaptionOpenMessageParameter Param { get; }
        }

        internal class VideoOpenMessage : MessageBase
        {
            public VideoOpenMessage(object sender, Megazone.Cloud.Media.Domain.Video video) : base(sender)
            {
                VideoParam = video;
            }

            public Megazone.Cloud.Media.Domain.Video VideoParam { get; }
        }
    }
}
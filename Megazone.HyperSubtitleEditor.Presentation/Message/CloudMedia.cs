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
    }
}
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class MediaPlayer
    {
        internal class PlayForwardByHalfSecondMessage : MessageBase
        {
            public PlayForwardByHalfSecondMessage(object sender) : base(sender)
            {
            }
        }

        internal class PlayBackByHalfSecondMessage : MessageBase
        {
            public PlayBackByHalfSecondMessage(object sender) : base(sender)
            {
            }
        }

        internal class PlayOrPauseMessage : MessageBase
        {
            public PlayOrPauseMessage(object sender) : base(sender)
            {
            }
        }

        internal class OpenLocalMediaMessage : MessageBase
        {
            public OpenLocalMediaMessage(object sender) : base(sender)
            {
            }
        }

        internal class OpenMediaFromUrlMessage : MessageBase
        {
            public OpenMediaFromUrlMessage(object sender, string url) : base(sender)
            {
                Url = url;
            }

            public string Url { get; }
        }
    }
}
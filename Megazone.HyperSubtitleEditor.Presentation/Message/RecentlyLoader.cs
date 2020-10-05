using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal class RecentlyLoader
    {
        internal class ChangeItemMessage : MessageBase
        {
            public ChangeItemMessage(object sender) : base(sender)
            {
            }
        }
    }
}
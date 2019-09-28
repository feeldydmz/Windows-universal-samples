using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class LeftSideMenu
    {
        internal class OpenMessage: MessageBase
        {
            public OpenMessage(object sender) : base(sender)
            {
            }
        }

        internal class CloseMessage : MessageBase
        {
            public CloseMessage(object sender) : base(sender)
            {
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    class RecentlyLoader
    {
        internal class ChangeItemMessage : MessageBase
        {
            public ChangeItemMessage(object sender) : base(sender)
            {
            }
        }
    }
}

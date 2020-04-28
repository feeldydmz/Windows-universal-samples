using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.View
{
    internal class CaptionElementsEditView 
    {
        internal class ChangedTabMessage : MessageBase
        {
            internal ChangedTabMessage(object sender ) : base(sender)
            {

            }
        }
    }
}

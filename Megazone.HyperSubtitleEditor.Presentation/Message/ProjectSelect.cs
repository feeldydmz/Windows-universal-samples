using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal class ProjectSelect 
    {
        internal class ProjectChangeMessage: MessageBase
        {
            public ProjectChangeMessage(object sender) : base(sender)
            {

            }
        }
    }
}

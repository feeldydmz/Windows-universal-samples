using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{

    internal class CaptionElementUpdateMessageParameter
    {
        internal CaptionElementUpdateMessageParameter(List<KeyValuePair<Caption, SourceLocationKind>> openCaptionElements,
            List<SubtitleTabItemViewModel> closeCaptionElements)
        {
            OpenCaptionElements = openCaptionElements;
            CloseCaptionElements = closeCaptionElements;
        }

        public List<KeyValuePair<Caption, SourceLocationKind>> OpenCaptionElements { get; set; }
        public List<SubtitleTabItemViewModel> CloseCaptionElements { get; set; }
    }
}

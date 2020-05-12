using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    public class CaptionOpenRequestedByIdMessageParameter
    {
        public CaptionOpenRequestedByIdMessageParameter( string videoId, string captionAssetId, IEnumerable<string> captionElements)
        {
            VideoId = videoId;
            CaptionAssetId = captionAssetId;
            CaptionElements = captionElements;
        }

        public string VideoId { get; }
        public string CaptionAssetId { get; }
        public IEnumerable<string> CaptionElements { get; }
    }
}

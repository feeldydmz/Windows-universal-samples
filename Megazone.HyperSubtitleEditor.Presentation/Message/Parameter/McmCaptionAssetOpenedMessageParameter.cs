using Megazone.Cloud.Media.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    public class McmCaptionAssetOpenedMessageParameter
    {
        public McmCaptionAssetOpenedMessageParameter(Video video, Asset<Caption> asset, IEnumerable<Caption> captions)
        {
            Video = video;
            Asset = asset;
            Captions = captions;
        }

        public Video Video { get; }
        public Asset<Caption> Asset { get; }
        public IEnumerable<Caption> Captions { get; }
    }
}

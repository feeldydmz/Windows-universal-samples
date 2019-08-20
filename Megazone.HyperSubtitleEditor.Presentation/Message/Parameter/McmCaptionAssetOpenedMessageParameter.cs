using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    public class McmCaptionAssetOpenedMessageParameter
    {
        public McmCaptionAssetOpenedMessageParameter(Video video, CaptionAsset asset, IEnumerable<Caption> captions)
        {
            Video = video;
            Asset = asset;
            Captions = captions;
        }

        public Video Video { get; }
        public CaptionAsset Asset { get; }
        public IEnumerable<Caption> Captions { get; }
    }
}

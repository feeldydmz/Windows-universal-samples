using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    public class CaptionOpenMessageParameter
    {
        public CaptionOpenMessageParameter(Video video, CaptionAsset asset, IEnumerable<Caption> captions,
            bool isOnline)
        {
            Video = video;
            Asset = asset;
            Captions = captions;
            IsOnline = isOnline;
        }

        public Video Video { get; }
        public CaptionAsset Asset { get; }
        public IEnumerable<Caption> Captions { get; }
        public bool IsOnline { get; }
    }
}
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    public class DeployRequestedMessageParameter
    {
        public DeployRequestedMessageParameter(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions)
        {
            Video = video;
            CaptionAsset = captionAsset;
            Captions = captions;
        }

        public Video Video { get; }
        public CaptionAsset CaptionAsset { get; }
        public IEnumerable<Caption> Captions { get; }
    }
}
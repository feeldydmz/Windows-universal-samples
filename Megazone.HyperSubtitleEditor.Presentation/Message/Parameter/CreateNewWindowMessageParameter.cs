using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    public class CreateNewWindowMessageParameter
    {
        public CreateNewWindowMessageParameter(
            Video videoInfo,
            CaptionAsset caption,
            List<Caption> captionAssetElements
        )
        {
            VideoInfo = videoInfo;
            Caption = caption;
            CaptionAssetElements = captionAssetElements;
        }

        public Video VideoInfo { get; }
        public CaptionAsset Caption { get; }
        public List<Caption> CaptionAssetElements { get; }
    }
}
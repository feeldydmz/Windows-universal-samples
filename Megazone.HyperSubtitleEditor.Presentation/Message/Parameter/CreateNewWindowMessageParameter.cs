using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

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

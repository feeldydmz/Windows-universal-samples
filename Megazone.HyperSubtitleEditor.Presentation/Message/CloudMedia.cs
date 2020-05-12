using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class CloudMedia
    {
        internal class CaptionOpenRequestedMessage : MessageBase
        {
            public CaptionOpenRequestedMessage(object sender, CaptionOpenMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public CaptionOpenMessageParameter Param { get; }
        }
        internal class CaptionResetMessage : MessageBase
        {
            public CaptionResetMessage(object sender) : base(sender)
            {
         
            }
        }

        internal class VideoOpenRequestedMessage : MessageBase
        {
            public VideoOpenRequestedMessage(object sender, Video video) : base(sender)
            {
                VideoParam = video;
            }

            public Video VideoParam { get; }
        }

        internal class DeployRequestedMessage : MessageBase
        {
            public DeployRequestedMessage(object sender, DeployRequestedMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public DeployRequestedMessageParameter Param { get; }
        }

        internal class CaptionAssetRenameRequestedMessage : MessageBase
        {
            public CaptionAssetRenameRequestedMessage(object sender, CaptionAsset captionAsset, string name) :
                base(sender)
            {
                CaptionAsset = captionAsset;
                Name = name;
            }

            public CaptionAsset CaptionAsset { get; }
            public string Name { get; }
        }

        internal class CaptionOpenRequestedByIdMessage : MessageBase
        {
            public CaptionOpenRequestedByIdMessage(object sender, CaptionOpenRequestedByIdMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public CaptionOpenRequestedByIdMessageParameter Param { get; }
        }
    }
}
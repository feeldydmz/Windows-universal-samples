using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class SignIn
    {
        internal class LogoutMessage : MessageBase
        {
            public LogoutMessage(object sender) : base(sender)
            {
            }
        }

        internal class CreateSignInViewMessage : MessageBase
        {
            public CreateSignInViewMessage(object sender) : base(sender)
            {
            }
        }

        internal class LoadStageProjectMessage : MessageBase
        {
            public LoadStageProjectMessage(object sender, UserProfile userProfile, bool isStartForStandAlone) :
                base(sender)
            {
                UserProfile = userProfile;
                IsStartForStandAlone = isStartForStandAlone;
            }

            public UserProfile UserProfile { get; }

            public bool IsStartForStandAlone { get; }
        }
    }
}
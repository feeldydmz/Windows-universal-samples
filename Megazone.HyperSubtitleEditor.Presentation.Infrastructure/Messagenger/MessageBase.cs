namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger
{
    public abstract class MessageBase : IMessage
    {
        protected MessageBase(object sender)
        {
            Sender = sender;
        }

        public object Sender { get; }
    }
}
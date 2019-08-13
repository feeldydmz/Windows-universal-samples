namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger
{
    public delegate void OnMessageReceived<in T>(T message) where T : IMessage;
}
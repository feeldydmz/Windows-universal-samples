namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger
{
    public delegate TResponse RequestToResponse<in TMessage, out TResponse>(TMessage message) where TMessage : IMessage;
}
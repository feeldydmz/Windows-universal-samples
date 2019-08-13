using System.Windows;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View
{
    public class ButtonInfo
    {
        public ButtonInfo(MessageBoxResult resultType,
            string content,
            ButtonRunTypes buttonRunType = ButtonRunTypes.None)
        {
            Content = content;
            ResultType = resultType;
            ButtonRunType = buttonRunType;
        }

        public MessageBoxResult ResultType { get; }

        public string Content { get; }

        public ButtonRunTypes ButtonRunType { get; }
    }
}
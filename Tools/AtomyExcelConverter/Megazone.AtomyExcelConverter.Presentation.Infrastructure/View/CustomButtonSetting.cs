using System.Windows;

namespace Megazone.AtomyExcelConverter.Presentation.Infrastructure.View
{
    public class CustomButtonSetting
    {
        public CustomButtonSetting(MessageBoxResult result, string content, ButtonRunTypes runType = ButtonRunTypes.None)
        {
            RunType = runType;
            Content = content;
            Result = result;
        }

        public ButtonRunTypes RunType { get; }
        public MessageBoxResult Result { get; }
        public string Content { get; }
    }
}

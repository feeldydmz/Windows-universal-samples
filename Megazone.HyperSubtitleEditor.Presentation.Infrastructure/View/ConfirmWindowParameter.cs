using System.Collections.Generic;
using System.Windows;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View
{
    public class ConfirmWindowParameter
    {
        public ConfirmWindowParameter(string windowTitle, string message, MessageBoxButton buttonType,
            Window winOwner = null,
            TextAlignment textAlignment = TextAlignment.Center
            )
        {
            Message = message;
            WindowTitle = windowTitle;
            ButtonType = buttonType;
            WindowOwner = winOwner;
            TextAlignment = textAlignment;
        }

        public string Message { get; }
        public MessageBoxButton ButtonType { get; }

        public string WindowTitle { get; }

        public ConfirmWindowCheckBoxParameter ConfirmWindowCheckBoxParameter { get; set; }

        public Window WindowOwner { get; set; }

        public IList<CustomButtonSetting> ButtonContents { get; set; }

        public TextAlignment TextAlignment { get; set; }
    }
}
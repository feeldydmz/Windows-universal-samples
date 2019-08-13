using System;
using System.Windows.Input;
using System.Windows.Markup;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Command.UI
{
    public class KeyGestureMarkup : MarkupExtension
    {
        public Key Key { get; set; }

        public bool Contorl { get; set; }

        public bool Shift { get; set; }

        public bool Alt { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var modifierKeys = ModifierKeys.None;
            if (Contorl)
                modifierKeys |= ModifierKeys.Control;
            if (Shift)
                modifierKeys |= ModifierKeys.Shift;
            if (Alt)
                modifierKeys |= ModifierKeys.Alt;

            return new KeyGesture(Key, modifierKeys);
        }
    }
}
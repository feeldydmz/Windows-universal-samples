using System;
using System.Globalization;
using System.Windows.Data;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

namespace Megazone.HyperSubtitleEditor.Presentation.Converter
{
    internal class OverMaxLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            return ConfigHolder.Current.Subtitle.MaxLines < (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
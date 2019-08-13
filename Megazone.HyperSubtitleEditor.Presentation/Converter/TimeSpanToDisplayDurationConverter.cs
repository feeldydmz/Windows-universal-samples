using System;
using System.Globalization;
using System.Windows.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.Converter
{
    internal class TimeSpanToDisplayDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan == false)
                return string.Empty;
            var timeSpan = (TimeSpan) value;
            var seconds = (int) timeSpan.TotalSeconds;
            return $"{seconds}.{timeSpan.Milliseconds:000}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.Converter
{
    internal class PlayDurationTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var totalPlaySeconds = (int) (decimal) value;
            if (totalPlaySeconds <= 0)
                totalPlaySeconds = 0;

            int hour = totalPlaySeconds / (60 * 60),
                minute = (totalPlaySeconds - hour * 60 * 60) / 60,
                second = totalPlaySeconds % 60;

            return $"{hour:00}:{minute:00}:{second:00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.Converter
{
    internal class InsertSharpAtTheBeginningToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? string.Empty : $"#{value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
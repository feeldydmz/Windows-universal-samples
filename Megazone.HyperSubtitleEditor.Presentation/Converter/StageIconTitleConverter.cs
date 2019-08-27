using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

namespace Megazone.HyperSubtitleEditor.Presentation.Converter
{
    internal class StageIconTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var name = value as string;

            return !string.IsNullOrEmpty(name)? name.Substring(0, 1) : "E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

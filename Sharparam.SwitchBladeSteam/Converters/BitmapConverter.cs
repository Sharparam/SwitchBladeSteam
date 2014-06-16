using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.Converters
{
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : Helper.BitmapToSource((Bitmap)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

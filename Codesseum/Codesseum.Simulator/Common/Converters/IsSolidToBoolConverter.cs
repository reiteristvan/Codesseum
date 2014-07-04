using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Codesseum.Simulator.Common.Converters
{
    public class IsSolidToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.LightSkyBlue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

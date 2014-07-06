using System;
using System.Windows.Data;

namespace Codesseum.Simulator.Common.Converters
{
    class HealthToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var health = (int) values[0];
            var maxHealth = (int) values[1];

            if (health <= 0)
            {
                return (double) 0;
            }

            return System.Convert.ToDouble(health == 0 || maxHealth == 0 ? 0 : (20/maxHealth)*health);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

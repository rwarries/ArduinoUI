using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApplication1
{
    public class ChangedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

            int number = (int)value;
            if (number > 10) return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            if (number > 7) return new SolidColorBrush(Color.FromArgb(20, 60,  0, 0));
            if (number > 5) return new SolidColorBrush(Color.FromArgb(50, 100, 0, 0));
            if (number > 4) return new SolidColorBrush(Color.FromArgb(100, 140, 0, 0));
            if (number > 3) return new SolidColorBrush(Color.FromArgb(150, 180, 0, 0));
            if (number > 2) return new SolidColorBrush(Color.FromArgb(200, 220, 0, 0));
            if (number > 1) return new SolidColorBrush(Color.FromArgb(255, 250, 0, 0));
            return new SolidColorBrush(Color.FromArgb(255,255,0,0));
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

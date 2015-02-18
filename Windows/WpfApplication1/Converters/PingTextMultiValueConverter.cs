using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApplication1
{
    public class PingTextMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            {
                if (values[0] == null || values[1] == null) return "invalid";

                bool isPingActive = (bool)values[0];
                string value = (string)values[1];


                if (isPingActive)
                {
                    // If it can be parsed as a string it is a time in ms else it is a "text-like" status...
                    try
                    {
                        int number = System.Convert.ToInt32(value);  //Need prefix System here...
                        return " " + number + " ms";
                    }
                    catch (Exception)
                    {
                        return " " + value;
                    }
                }
                else
                {
                    return " ?";
                }
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


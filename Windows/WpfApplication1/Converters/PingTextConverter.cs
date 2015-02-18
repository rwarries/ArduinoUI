using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApplication1
{
    public class PingTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is String)
            {
                string result = (String) value;
                int number;
                // If it can be parsed as a string it is a time in ms else it is a status...
                try{
                    number = System.Convert.ToInt32(result);  //Need prefix System here...
                    return " " + result + " ms";
                }
                catch (Exception){
                    return " " + result;
                }
            }
            else
            {
                throw new ArgumentNullException("Conver cannot handle null. It needs as string.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}

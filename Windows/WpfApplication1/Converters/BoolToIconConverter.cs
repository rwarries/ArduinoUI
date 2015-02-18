using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace WpfApplication1
{

    public class BoolToIconConverter : BaseConverter, IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if ((value == null) || !(value is bool))
                return null;

            if ((bool)value){
                return new BitmapImage(new Uri("arrow-090.png", UriKind.Relative));
            }
            else{
                return new BitmapImage(new Uri("arrow-270.png", UriKind.Relative));
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
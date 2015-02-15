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
    /**
     * Convert an array of bytes to Ascii as far as possible
     */

    public class BinaryByteBufferToAsciiStringConverter : BaseConverter, IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {

            StringBuilder result = new StringBuilder();
            if ((value == null) || !(value is RingBuffer<byte>))
            {
                return null;
            }

            RingBuffer<byte> inBytes = (RingBuffer<byte>)value;
            for (int i = 0; i < inBytes.Count;i++ ){
                if (inBytes[i]<32 || inBytes[i]>126){
                    result.Append(".");
                }
                else {
                    result.Append((char) inBytes[i]);
                }
            }
            return result.ToString();
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
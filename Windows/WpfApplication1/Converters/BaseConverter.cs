using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

// Neat way to reduce clutter in xaml (no need to use resource and key for converter @see http://www.wpftutorial.net/ValueConverters.html
namespace WpfApplication1
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SquirrelApp
{
    //[ValueConversion(typeof(ComboBoxItem), typeof(string))]
    public class ComboBoxItemConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ComboBoxItem cb = (ComboBoxItem)value;
            return cb.Content;
        }
    }
}

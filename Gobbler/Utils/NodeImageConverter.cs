using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gobbler
{
    [ValueConversion(typeof(NodeKind), typeof(string))]
    public class NodeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NodeKind kind = (NodeKind)value;
            string imgfile = string.Format("Images/Node{0}.png", kind.ToString());
            return imgfile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
   
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ViewModelLib
{
    public class ViewModelSchemaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<IViewModel> vms;
            vms = value as IEnumerable<IViewModel>;
            if (vms == null) return null;
            return new ViewModelSchema(vms,vms.FirstOrDefault()?.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

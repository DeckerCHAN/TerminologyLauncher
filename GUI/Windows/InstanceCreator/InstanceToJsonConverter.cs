using System;
using System.Globalization;
using System.Windows.Data;

namespace TerminologyLauncher.GUI.Windows.InstanceCreator
{
    public class InstanceToJsonConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Utils.SerializeUtils.JsonConverter.ConvertToJson(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
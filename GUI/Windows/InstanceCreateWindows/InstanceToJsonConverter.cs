using System;
using System.Globalization;
using System.Windows.Data;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.GUI.Windows.InstanceCreateWindows
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
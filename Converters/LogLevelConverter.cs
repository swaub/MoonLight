using System.Globalization;
using System.Windows.Data;

namespace MoonLight.Converters
{
    public class LogLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string logEntry)
            {
                if (logEntry.Contains("[WARNING]"))
                    return "WARNING";
                if (logEntry.Contains("[ERROR]"))
                    return "ERROR";
                if (logEntry.Contains("[DEBUG]"))
                    return "DEBUG";
            }
            return "INFO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
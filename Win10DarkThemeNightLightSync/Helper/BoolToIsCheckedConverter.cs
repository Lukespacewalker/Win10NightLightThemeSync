using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Win10DarkThemeNightLightSync.Helper
{
    public class BoolToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == (bool)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return (bool)parameter;
            }
            return Binding.DoNothing;
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool @bool)
            {
                return @bool ? Visibility.Visible : Visibility.Collapsed;
            }
            else throw new ArgumentException("value type must be boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
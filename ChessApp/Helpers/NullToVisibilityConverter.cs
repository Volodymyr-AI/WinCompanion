using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Helpers;

public class NullToVisibilityConverter : IValueConverter
{
    // Converts the input value to a Visibility value (Collapsed if null, Visible otherwise)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is null ? Visibility.Collapsed : Visibility.Visible;

    // ConvertBack is not implemented because we don't need to convert from Visibility back to the original value
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
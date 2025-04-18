using System.Windows;

namespace ChessApp.Infrastructure.Log;

public class Logging
{
    public static void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public static void ShowInfo(string message)
    {
        MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
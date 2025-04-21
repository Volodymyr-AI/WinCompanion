using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ChessApp.Models.Board;

namespace ChessApp.Infrastructure.Helpers;

public class SelectionToBrushConverter : IValueConverter
{
    // Converts the input value (ChessSquare) to a Brush based on the selection state
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if the value is of type ChessSquare
        if (value is ChessSquare square)
        {
            // If the square is selected, return a green brush (to highlight the selection)
            if (square.IsSelected)
                return square.IsSelected ? Brushes.Green : Brushes.Transparent;
            
            // If not selected, return the square's original background color
            return square.Background ?? Brushes.Transparent;
        }

        // If the value is not a ChessSquare, return a transparent brush
        return Brushes.Transparent;
    }

    // ConvertBack is not implemented because we don't need to convert from Brush back to ChessSquare
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
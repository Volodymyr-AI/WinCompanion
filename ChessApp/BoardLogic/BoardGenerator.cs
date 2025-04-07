using System.Windows.Media;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic;

public class BoardGenerator
{
    public static IEnumerable<ChessSquare> GenerateSquares()
    {
        bool isWhite = true;
        for (int row = 0; row < 8; row++)
        {
            isWhite = row % 2 == 0;
            for (int col = 0; col < 8; col++)
            {
                yield return new ChessSquare
                {
                    Row = row,
                    Column = col,
                    Background = isWhite ? Brushes.White : Brushes.Gray
                };
                isWhite = !isWhite;
            }
        }
    }
}
using System.Windows.Media;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Board;

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
                var baseColor = isWhite ? Brushes.WhiteSmoke : Brushes.SlateGray;
                yield return new ChessSquare
                {
                    Row = row,
                    Column = col,
                    Background = baseColor,
                    BaseBackground = baseColor
                };
                isWhite = !isWhite;
            }
        }
    }
}
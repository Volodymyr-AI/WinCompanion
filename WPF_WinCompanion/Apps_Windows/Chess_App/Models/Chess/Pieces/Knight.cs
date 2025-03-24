using System.Collections.ObjectModel;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

/// <summary>
/// Represents a knight chess piece.
/// </summary>
public class Knight : ChessPiece
{
    /// <summary>
    /// The type of the piece - Knight.
    /// </summary>
    public override PieceType Type => PieceType.Knight;
    /// <summary>
    /// Checks whether the move is valid for a knight.
    /// </summary>
    /// <param name="from">The square from which the piece moves.</param>
    /// <param name="to">The square to which the piece moves.</param>
    /// <param name="board">The current chessboard as a collection of squares.</param>
    /// <returns>Returns true if the move is valid, otherwise false.</returns>
    public override bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board)
    {
        int rowDiff = Math.Abs(from.Row - to.Row);
        int colDiff = Math.Abs(from.Column - to.Column);

        // Knight moves in an "L" shape: (2 by 1 or 1 by 2)
        bool isKnightMove = (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);

        // Knight can jump over pieces, so we only check destination square
        return isKnightMove && (to.Piece == null || to.Piece.Color != this.Color);
    }
}
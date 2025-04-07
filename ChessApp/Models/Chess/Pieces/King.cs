using System.Collections.ObjectModel;
using ChessApp.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

/// <summary>
/// Represents a king chess piece.
/// </summary>
public class King : ChessPiece
{
    /// <summary>
    /// The type of the piece - King.
    /// </summary>
    public override PieceType Type => PieceType.King;
    
    public bool HasMoved { get; set; } = false;
    
    /// <summary>
    /// Checks whether the move is valid for a king.
    /// </summary>
    /// <param name="from">The square from which the piece moves.</param>
    /// <param name="to">The square to which the piece moves.</param>
    /// <param name="board">The current chessboard as a collection of squares.</param>
    /// <returns>Returns true if the move is valid, otherwise false.</returns>
    public override bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board)
    {
        int rowDiff = Math.Abs(from.Row - to.Row);
        int colDiff = Math.Abs(from.Column - to.Column);

        // The king moves exactly one square in any direction
        bool isKingMove = rowDiff <= 1 && colDiff <= 1 && (rowDiff + colDiff != 0);

        if (!HasMoved && rowDiff == 0 && Math.Abs(colDiff) == 2)
        {
            return true;
        }

        // Check if the destination square is empty or occupied by an opponent's piece
        return isKingMove && (to.Piece == null || to.Piece.Color != this.Color);
    }
}
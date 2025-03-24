using System.Collections.ObjectModel;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

/// <summary>
/// Represents a pawn chess piece.
/// </summary>
public class Pawn : ChessPiece
{
    /// <summary>
    /// The type of the piece - Pawn.
    /// </summary>
    public override PieceType Type => PieceType.Pawn;

    /// <summary>
    /// Checks whether the move is valid for a pawn.
    /// </summary>
    /// <param name="from">The square from which the piece moves.</param>
    /// <param name="to">The square to which the piece moves.</param>
    /// <param name="board">The current chessboard as a collection of squares.</param>
    /// <returns>Returns true if the move is valid, otherwise false.</returns>
    public override bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board)
    {
        int direction = Color == PieceColor.White ? -1 : 1; // Determines pawn's movement direction
        int startRow = Color == PieceColor.White ? 6 : 1; // Starting row for pawns

        // One square forward (only if the destination square is empty)
        if (to.Column == from.Column && 
            to.Row == from.Row + direction && 
            to.Piece == null)
            return true;

        // Two squares forward from the starting position
        if (from.Row == startRow &&
            to.Column == from.Column &&
            to.Row == from.Row + (2 * direction) &&
            board.FirstOrDefault(s => s.Row == from.Row + direction && s.Column == from.Column)?.Piece == null &&
            to.Piece == null)
        {
            return true;
        }

        // Capturing diagonally
        if (to.Piece != null &&
            to.Piece.Color != Color &&
            Math.Abs(to.Column - from.Column) == 1 &&
            to.Row == from.Row + direction)
        {
            return true;
        }

        return false;
    }
}
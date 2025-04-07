using System.Collections.ObjectModel;
using System.Windows.Controls;
using ChessApp.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

public class Rook : ChessPiece
{
    /// <summary>
    /// The type of the piece - Rook.
    /// </summary>
    public override PieceType Type => PieceType.Rook;
    
    public bool HasMoved { get; set; } = false;
    /// <summary>
    /// Checks whether the move is valid for a rook.
    /// </summary>
    /// <param name="from">The square from which the piece moves.</param>
    /// <param name="to">The square to which the piece moves.</param>
    /// <param name="board">The current chessboard as a collection of squares.</param>
    /// <returns>Returns true if the move is valid, otherwise false.</returns>
    public override bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board)
    {
        // The rook can only move vertically or horizontally
        if (from.Row != to.Row && from.Column != to.Column)
            return false;

        // Check if the path is clear
        if (from.Row == to.Row)
        {
            // Move horizontally
            int step = (to.Column > from.Column) ? 1 : -1;
            for (int col = from.Column + step; col != to.Column; col += step)
            {
                if (board.Any(sq => sq.Row == from.Row && sq.Column == col && sq.Piece != null))
                    return false;
            }
        }
        else if (from.Column == to.Column)
        {
            // Move vertically
            int step = (to.Row > from.Row) ? 1 : -1;
            for (int row = from.Row + step; row != to.Row; row += step)
            {
                if (board.Any(sq => sq.Row == row && sq.Column == from.Column && sq.Piece != null))
                    return false;
            }
        }

        if (!HasMoved)
        {
            return from.Row == to.Row || from.Column == to.Column;
        }

        // Check if the destination square is empty or occupied by an opponent's piece
        return to.Piece == null || to.Piece.Color != this.Color;
    }
}
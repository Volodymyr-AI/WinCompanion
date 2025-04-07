using System.Collections.ObjectModel;
using ChessApp.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

/// <summary>
/// Represents a bishop chess piece.
/// </summary>
public class Bishop : ChessPiece
{
    /// <summary>
    /// The type of the piece - Bishop.
    /// </summary>
    public override PieceType Type => PieceType.Bishop;
    /// <summary>
    /// Checks whether the move is valid for a bishop.
    /// </summary>
    /// <param name="from">The square from which the piece moves.</param>
    /// <param name="to">The square to which the piece moves.</param>
    /// <param name="board">The current chessboard as a collection of squares.</param>
    /// <returns>Returns true if the move is valid, otherwise false.</returns>
    public override bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board)
    {
        // The bishop moves diagonally, so the absolute difference between rows and columns must be the same
        if(Math.Abs(from.Row - to.Row) != Math.Abs(from.Column - to.Column))
            return false;
        
        // Determine the direction of movement for rows and columns
        int rowDirection = to.Row > from.Row ? 1 : -1;
        int colDirection = to.Column > from.Column ? 1 : -1;
        
        int currentRow = from.Row + rowDirection;
        int currentCol = from.Column + colDirection;

        // Check if any piece blocks the path
        while (currentRow != to.Row && currentCol != to.Column)
        {
            if (board.Any(sq => sq.Row == currentRow && sq.Column == currentCol && sq.Piece != null))
            {
                return false;
            }
            
            currentRow += rowDirection;
            currentCol += colDirection;
        }
        
        // Check if the destination square is empty or occupied by an opponent's piece
        return to.Piece == null || to.Piece.Color != this.Color;
    }
}
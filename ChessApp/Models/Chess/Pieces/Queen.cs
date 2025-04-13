using System.Collections.ObjectModel;
using ChessApp.Models.Board;

namespace ChessApp.Models.Chess.Pieces;

/// <summary>
/// Represents a queen chess piece.
/// </summary>
public class Queen : ChessPiece
{
    /// <summary>
    /// The type of the piece - Queen.
    /// </summary>
    public override PieceType Type => PieceType.Queen;
    
    /// <summary>
    /// Checks whether the move is valid for a queen.
    /// </summary>
    /// <param name="from">The square from which the piece moves.</param>
    /// <param name="to">The square to which the piece moves.</param>
    /// <param name="board">The current chessboard as a collection of squares.</param>
    /// <returns>Returns true if the move is valid, otherwise false.</returns>
    public override bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board)
    {
        int rowDiff = Math.Abs(from.Row - to.Row);
        int colDiff = Math.Abs(from.Column - to.Column);

        // The queen moves diagonally, vertically, or horizontally
        if (rowDiff != colDiff && from.Row != to.Row && from.Column != to.Column)
            return false;

        int rowDirection = to.Row > from.Row ? 1 : (to.Row < from.Row ? -1 : 0);
        int colDirection = to.Column > from.Column ? 1 : (to.Column < from.Column ? -1 : 0);

        int currentRow = from.Row + rowDirection;
        int currentCol = from.Column + colDirection;

        // Check if any piece blocks the path
        while (currentRow != to.Row || currentCol != to.Column)
        {
            if (board.Any(sq => sq.Row == currentRow && sq.Column == currentCol && sq.Piece != null))
                return false;

            currentRow += rowDirection;
            currentCol += colDirection;
        }

        // The destination square must be empty or occupied by an opponent's piece
        return to.Piece == null || to.Piece.Color != this.Color;
    }
}
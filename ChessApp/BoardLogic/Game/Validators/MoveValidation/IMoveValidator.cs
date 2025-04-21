using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Validators.MoveValidation;

public interface IMoveValidator
{
    /// <summary>
    /// Checks if move is valid
    /// </summary>
    bool IsMoveValid(
        ChessBoardModel board, 
        ChessSquare from, 
        ChessSquare to, 
        PieceColor turn, 
        out string errorMessage);
}
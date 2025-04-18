using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Validators.MoveValidation;

public class MoveValidator : IMoveValidator
{
    public bool IsMoveValid(ChessBoardModel board, ChessSquare from, ChessSquare to, PieceColor turn)
    {
        throw new NotImplementedException();
    }
}
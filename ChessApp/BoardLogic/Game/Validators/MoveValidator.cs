using ChessApp.BoardLogic.Interfaces;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Validators;

public class MoveValidator : IMoveValidator
{
    public bool IsMoveValid(ChessBoardModel board, ChessSquare from, ChessSquare to, PieceColor turn)
    {
        throw new NotImplementedException();
    }
}
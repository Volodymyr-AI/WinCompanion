using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Interfaces;

public interface IMoveValidator
{
    bool IsMoveValid(ChessBoardModel board, ChessSquare from, ChessSquare to, PieceColor turn);
}
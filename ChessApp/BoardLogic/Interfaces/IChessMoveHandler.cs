using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Interfaces;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    void OnSquareClicked(ChessSquare clickedSquare);
}
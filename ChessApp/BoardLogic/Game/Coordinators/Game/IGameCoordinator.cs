using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Coordinators.Game;

public interface IGameCoordinator
{
    void OnSquareClicked(ChessSquare clickedSquare);
}
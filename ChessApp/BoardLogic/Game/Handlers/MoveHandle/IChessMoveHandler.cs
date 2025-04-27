using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Handlers.MoveHandle;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    void HandlePieceMovement(ChessSquare destination);
    
}
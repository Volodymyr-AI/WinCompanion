using ChessApp.Models.Board;
using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.BoardLogic.Game.Handlers.MoveHandle;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    event Action<Move> MoveExecuted;
    void HandlePieceMovement(ChessSquare destination);
    
}
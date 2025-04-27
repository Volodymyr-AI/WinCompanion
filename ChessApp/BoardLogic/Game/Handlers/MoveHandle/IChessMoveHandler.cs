using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Handlers.MoveHandle;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    
    ChessSquare? SelectedSquare { get; }
    bool HasSelectedPiece { get; }
    
    void SelectPiece(ChessSquare selectedSquare);
    void UnselectPiece(ChessSquare selectedSquare);
    void HandlePieceMovement(ChessSquare destination);
    
}
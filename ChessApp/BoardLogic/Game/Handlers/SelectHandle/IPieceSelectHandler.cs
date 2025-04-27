using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Handlers.SelectHandle;

public interface IPieceSelectHandler
{
    void SelectPiece(ChessSquare selectedSquare);
    void UnselectPiece(ChessSquare selectedSquare);
    
    ChessSquare? SelectedSquare { get; }
    bool HasSelectedPiece { get; }
    void SetSelectedSquareToNull();
}
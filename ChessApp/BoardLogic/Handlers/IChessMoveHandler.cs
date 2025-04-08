using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace ChessApp.BoardLogic.Handlers;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    void OnSquareClicked(ChessSquare clickedSquare);
}
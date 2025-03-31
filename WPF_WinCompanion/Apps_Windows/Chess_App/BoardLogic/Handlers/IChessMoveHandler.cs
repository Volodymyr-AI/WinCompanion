using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    void OnSquareClicked(ChessSquare clickedSquare);
}
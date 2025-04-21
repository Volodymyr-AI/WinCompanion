using System.ComponentModel;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Handlers.GameHandle;

public interface IGameHandler : INotifyPropertyChanged
{
    /// <summary> Colour that must move now </summary>
    PieceColor CurrentTurn { get; }

    /// <summary> Fires after legal move </summary>
    event Action GameUpdated;

    /// <summary> Calls once a game starts or by "Restart" </summary>
    void RestartGame();

    /// <summary> Checks the game position and returns <c>true</c>, if it already finished. </summary>
    bool CheckGameStatus();
}
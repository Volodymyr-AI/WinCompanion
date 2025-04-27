using System.ComponentModel;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Managers.GameManager;

public interface IGameStatusManager : INotifyPropertyChanged
{
    /// <summary> Colour that must move now </summary>
    PieceColor CurrentTurn { get; }
    /// <summary> Check if game is over </summary>
    bool IsGameOver { get; }
    ChessBoardModel BoardModel { get; }
    /// <summary> Fires after legal move </summary>
    event Action GameUpdated;
    /// <summary> Calls once a game starts or by "Restart" </summary>
    void RestartGame();
    /// <summary> Checks the game position and returns <c>true</c>, if it already finished. </summary>
    bool CheckGameStatus();
    void SwitchTurn();
    
    bool TrySetGameOver();
}
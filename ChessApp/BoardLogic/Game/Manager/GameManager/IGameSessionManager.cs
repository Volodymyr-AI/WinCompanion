using System.ComponentModel;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Manager.GameManager;

public interface IGameSessionManager : INotifyPropertyChanged
{
    /// <summary> Colour that must move now </summary>
    PieceColor CurrentTurn { get; }

    /// <summary> Fires after legal move </summary>
    event Action GameUpdated;

    /// <summary> Calls once a game starts or by "Restart" </summary>
    void RestartGame();

    /// <summary> Checks the game position and returns <c>true</c>, if it already finished. </summary>
    bool CheckGameStatus();
    
    /// <summary> Informs manager about move made </summary>
    void OnMoveMade();
    
    /// <summary> Check if game has ended </summary>
    bool IsGameOver { get; }
    
    PieceColor LastTurn { get; }
}
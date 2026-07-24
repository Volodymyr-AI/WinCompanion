using System.ComponentModel;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.BoardLogic.Game.Managers.GameManager;

public interface IGameStatusManager : INotifyPropertyChanged
{
    /// <summary> Colour that must move now </summary>
    PieceColor CurrentTurn { get; }
    /// <summary> Check if game is over </summary>
    bool IsGameOver { get; }
    /// <summary> Current counter for fifty-move rule </summary>
    int HalfMoveCounter { get; }
    ChessBoardModel BoardModel { get; }
    /// <summary> Fires after legal move </summary>
    event Action GameUpdated;
    /// <summary> Calls once a game starts or by "Restart" </summary>
    void RestartGame();
    /// <summary> Checks the game position and returns <c>true</c>, if it already finished. </summary>
    bool CheckGameStatus();
    void UpdateFiftyMoveRule(Move move);
    bool CanClaimFiftyMoveDraw();
    void SwitchTurn();
    
    bool TrySetGameOver();
}
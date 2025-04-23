using System.ComponentModel;
using System.Windows;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.BoardLogic.Game.Validators.StalemateValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Manager.GameManager;

/// <summary>
/// Central game flow coordinator: handles move turns,
/// listens for actions <see cref="IChessMoveHandler"/> and informs UI
/// </summary>
public sealed class GameSessionManager : IGameSessionManager
{
    #region ctor / fields --------------------------------------------------------------------
    private readonly ChessBoardModel _board;
    private readonly CastlingValidator _castling;
    private bool _isGameOver;
    
    public bool IsGameOver => _isGameOver;
    
    public GameSessionManager(
        ChessBoardModel board,
        CastlingValidator castling)
    {
        _board       = board;
        _castling    = castling;
    }
    
    #endregion
    
    private PieceColor _currentTurn = PieceColor.White;
    private PieceColor _lastTurn = PieceColor.Black;
    public PieceColor CurrentTurn
    {
        get => _currentTurn;
        set
        {
            if (_currentTurn != value)
            {
                _currentTurn = value;
                OnPropertyChanged(nameof(CurrentTurn));
            }
        }
    }
    public PieceColor LastTurn => _lastTurn;

    public event Action GameUpdated = delegate { };
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// Restart a game, moving all pieces back
    /// </summary>
    public void RestartGame()
    {
        ChessBoardInitializer.InitializeBoard(_board);
        _castling.Reset();

        _isGameOver = false;
        CurrentTurn = PieceColor.White;

        GameUpdated?.Invoke();
    }
    
    
    /// <summary>
    /// Check game status after every move
    /// </summary>
    public bool CheckGameStatus()
    {
        if (CheckMateValidator.IsKingCheck(_board, _currentTurn))
        {
            if (CheckMateValidator.IsCheckmate(_board, _currentTurn))
                return true;
        }
        else if (StalemateValidator.IsStalemate(_board, _currentTurn))
            return true;
        
        return false;
    }
    
    // Calls after every move, to check status of the game and change side
    public void OnMoveMade()
    {
        _lastTurn = _currentTurn;
        
        var gameEnded = CheckGameStatus();

        if (!gameEnded)
        {
            _lastTurn = _currentTurn;
            CurrentTurn = Opponent(_currentTurn);
        }

        GameUpdated.Invoke();
    }
    
    /// <summary>
    /// Get opponents color
    /// </summary>
    public PieceColor Opponent( PieceColor color)
        => color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    
    /// <summary>
    /// Notify UI about property change
    /// </summary>
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
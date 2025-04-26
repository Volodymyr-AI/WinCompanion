using System.ComponentModel;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.BoardLogic.Game.Validators.StalemateValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Game.Managers.GameManager;

/// <summary>
/// Central game flow coordinator: handles move turns,
/// listens for actions <see cref="IChessMoveHandler"/> and informs UI
/// </summary>
public sealed class GameStatusManager : IGameStatusManager
{
    #region ctor / fields --------------------------------------------------------------------
    private readonly ChessBoardModel _board;
    private readonly CastlingValidator _castling;
    private readonly IChessMoveHandler _moveHandler;
    
    public GameStatusManager(
        ChessBoardModel board, 
        IChessMoveHandler moveHandler, 
        CastlingValidator castling)
    {
        _board       = board;
        _moveHandler = moveHandler;
        _castling    = castling;

        _moveHandler.BoardUpdated += OnMoveMade;
    }
    
    #endregion
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
    private PieceColor _currentTurn = PieceColor.White;


    public event Action GameUpdated = delegate { };
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// Restart a game, moving all pieces back
    /// </summary>
    public void RestartGame()
    {
        ChessBoardInitializer.InitializeBoard(_board);
        _castling.Reset();
        
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
            {
                Logging.ShowInfo($"Checkmate! {_currentTurn} lost.");
                return true;
            }
            else
            {
                Console.WriteLine($"{_currentTurn} - King Check!");
                return false;
            }
        }
        else if (StalemateValidator.IsStalemate(_board, _currentTurn))
        {
            Logging.ShowInfo($"Game finished. Stalemate!");
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Calls after every move, to check status of the game and change side
    /// </summary>
    private void OnMoveMade()
    {
        if (!CheckGameStatus())
        {
            CurrentTurn = Opponent(CurrentTurn);
            GameUpdated.Invoke();
        }
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
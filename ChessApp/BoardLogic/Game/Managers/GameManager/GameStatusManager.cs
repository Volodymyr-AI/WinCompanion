using System.ComponentModel;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.BoardLogic.Game.Validators.EnPassantValidation;
using ChessApp.BoardLogic.Game.Validators.FiftyMoveRuleValidation;
using ChessApp.BoardLogic.Game.Validators.StalemateValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Services.PieceNotationService.Entity;

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
    private readonly FiftyMoveRuleValidator _fiftyMoveRule;
    private readonly EnPassantValidator _enPassant;
    
    public GameStatusManager(
        ChessBoardModel board, 
        CastlingValidator castling,
        EnPassantValidator enPassant)
    {
        _board       = board;
        _castling    = castling;
        _fiftyMoveRule = new FiftyMoveRuleValidator();
        _enPassant = enPassant;
    }
    
    #endregion
    
    public ChessBoardModel BoardModel => _board;
    
    private PieceColor _currentTurn = PieceColor.White;
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
    
    public bool IsGameOver { get; private set; }
    
    /// <summary>
    /// Current counter of 50 moves rule
    /// </summary>
    public int HalfMoveCounter => _fiftyMoveRule.HalfMoveCounter;
    
    public event Action GameUpdated = delegate { };
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// Restart a game, moving all pieces back
    /// </summary>
    public void RestartGame()
    {
        ChessBoardInitializer.InitializeBoard(_board);
        _castling.Reset();
        _enPassant.Reset();
        
        IsGameOver = false;
        CurrentTurn = PieceColor.White;
        GameUpdated?.Invoke();
    }

    /// <summary>
    /// Update state of a 50 move rule after move
    /// </summary>
    /// <param name="move"> Data about last made move </param>
    public void UpdateFiftyMoveRule(Move move)
    {
        _fiftyMoveRule.UpdateAfterMove(move);
    }
    
    /// <summary>
    /// Check game status after every move
    /// </summary>
    public bool CheckGameStatus()
    {
        if (_fiftyMoveRule.IsFiftyMoveRuleDraw())
        {
            IsGameOver = true;
            Logging.ShowInfo("Draw by fifty-move rule!");
            return true;
        }
        
        if (CheckMateValidator.IsKingCheck(_board, _currentTurn))
        {
            if (CheckMateValidator.IsCheckmate(_board, _currentTurn))
            {
                IsGameOver = true;
                return true;
            }
            
            return false;
        }
        else if (StalemateValidator.IsStalemate(_board, _currentTurn))
        {
            IsGameOver = true;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Check, if stalemate can be declared following 50 move rule
    /// </summary>
    /// <returns></returns>
    public bool CanClaimFiftyMoveDraw()
    {
        return _fiftyMoveRule.IsFiftyMoveRuleDraw() && !IsGameOver;
    }
    
    public void SwitchTurn()
    {
        CurrentTurn = Opponent(CurrentTurn);
        GameUpdated?.Invoke();
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

    public bool TrySetGameOver()
    {
        if (IsGameOver)
            return false;
        
        IsGameOver = true;
        return true;
    }

    /// <summary>
    /// Update en passant state after a move
    /// </summary>
    /// <param name="fromSquare"></param>
    /// <param name="toSquare"></param>
    /// <param name="moveNumber"></param>
    public void UpdateEnPassant(ChessSquare fromSquare, ChessSquare toSquare, int moveNumber)
    {
        _enPassant.UpdateAfterMove(_board, toSquare, fromSquare, moveNumber);
    }

    /// <summary>
    /// Get en passant validator for other components
    /// </summary>
    /// <returns></returns>
    public EnPassantValidator GetEnPassantValidator()
    {
        return _enPassant;
    }
}
using System.ComponentModel;
using System.Windows;
using ChessApp.BoardLogic.Validators;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.BoardLogic.Handlers;

public class GameHandler : INotifyPropertyChanged
{
    private readonly ChessBoardModel _boardModel;
    private readonly IChessMoveHandler _moveHandler;
    private PieceColor _currentTurn = PieceColor.White;
    private CastlingValidator _castlingValidator;

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

    public event Action GameUpdated;
    public event PropertyChangedEventHandler PropertyChanged;
    public GameHandler(ChessBoardModel boardModel, IChessMoveHandler moveHandler, CastlingValidator castlingValidator)
    {
        _boardModel = boardModel;
        _moveHandler = moveHandler;
        _currentTurn = PieceColor.White;
        _castlingValidator = castlingValidator;
        _moveHandler.BoardUpdated += OnMoveMade;
    }

    public GameHandler(CastlingValidator castlingValidator)
    {
        _castlingValidator = castlingValidator;
    }
    
    /// <summary>
    /// Check game status after every move
    /// </summary>
    public bool CheckGameStatus()
    {
        if (CheckMateValidator.IsKingCheck(_boardModel, _currentTurn))
        {
            if (CheckMateValidator.IsCheckmate(_boardModel, _currentTurn))
            {
                MessageBox.Show($"Checkmate! {_currentTurn} lost.");
                return true;
            }
            else
            {
                Console.WriteLine($"{_currentTurn} - King Check!");
                return false;
            }
        }
        else if (StalemateValidator.IsStalemate(_boardModel, _currentTurn))
        {
            MessageBox.Show($"Game finished. Stalemate!");
            return true;
        }
        return false;
    }
    /// <summary>
    /// Get opponents color
    /// </summary>
    public PieceColor Opponent( PieceColor color)
        => color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    
    /// <summary>
    /// Calls after every move, to check status of the game and change side
    /// </summary>
    private void OnMoveMade()
    {
        CheckGameStatus();
        CurrentTurn = Opponent(_currentTurn);
        
        GameUpdated?.Invoke();
    }
    
    /// <summary>
    /// Restart a game, moving all pieces back
    /// </summary>
    public void RestartGame()
    {
        ChessBoardInitializer.InitializeBoard(_boardModel);
        _castlingValidator.Reset();
        CurrentTurn = PieceColor.White;
        GameUpdated?.Invoke();
    }
    
    /// <summary>
    /// Notify UI about property change
    /// </summary>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
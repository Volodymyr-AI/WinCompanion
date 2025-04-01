using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class GameHandler : INotifyPropertyChanged
{
    private readonly ChessBoardModel _boardModel;
    private readonly IChessMoveHandler _moveHandler;
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

    public event Action GameUpdated;
    public event PropertyChangedEventHandler PropertyChanged;
    public GameHandler(ChessBoardModel boardModel, IChessMoveHandler moveHandler)
    {
        _boardModel = boardModel;
        _moveHandler = moveHandler;
        _currentTurn = PieceColor.White;
        
        _moveHandler.BoardUpdated += OnMoveMade;
    }
    
    /// <summary>
    /// Check game status after every move
    /// </summary>
    public void CheckGameStatus()
    {
        if (CheckMateValidator.IsKingCheck(_boardModel, _currentTurn))
        {
            if (CheckMateValidator.IsCheckmate(_boardModel, _currentTurn))
            {
                MessageBox.Show($"Checkmate! {_currentTurn} lost.");
                //Debug.WriteLine($"{_currentTurn} is checkmated!");
                return;
            }
            else
            {
                MessageBox.Show($"{_currentTurn} - King Check!");
                //Debug.WriteLine($"{_currentTurn} is in check!");
            }
        }
        else if (StalemateValidator.IsStalemate(_boardModel, _currentTurn))
        {
            MessageBox.Show($"Game finished. Stalemate!");
            //Debug.WriteLine("Game ended in stalemate!");
        }
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
        //Debug.WriteLine($"🟢 OnMoveMade called! CurrentTurn before: {_currentTurn}");
    
        CheckGameStatus();
        CurrentTurn = Opponent(_currentTurn);

        //Debug.WriteLine($"🔄 Turn switched to: {CurrentTurn}");
    
        GameUpdated?.Invoke();
    }
    
    /// <summary>
    /// Restart a game, moving all pieces back
    /// </summary>
    public void RestartGame()
    {
        ChessBoardInitializer.InitializeBoard(_boardModel);
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
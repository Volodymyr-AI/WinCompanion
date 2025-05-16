using System.ComponentModel;

namespace ChessApp.Services.GameHistory.Models;

public class MoveHistoryItem
{
    public int MoveNumber { get; }
    public string WhiteMove { get; }
    private string _blackMove;
    
    public string BlackMove
    {
        get => _blackMove;
        set
        {
            _blackMove = value;
            OnPropertyChanged(nameof(BlackMove));
        }
    }

    public MoveHistoryItem(int moveNumber, string whiteMove, string? blackMove)
    {
        MoveNumber = moveNumber;
        WhiteMove = whiteMove;
        _blackMove = blackMove;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
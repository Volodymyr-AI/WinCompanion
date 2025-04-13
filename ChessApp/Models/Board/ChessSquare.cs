using System.ComponentModel;
using System.Windows.Media;
using ChessApp.Models.Chess;

namespace ChessApp.Models.Board;

public class ChessSquare : INotifyPropertyChanged
{
    public int Row { get; set; }
    public int Column { get; set; }
    private Brush _background;
    
    public Brush Background
    {
        get => _background;
        set
        {
            _background = value;
            OnPropertyChanged(nameof(Background));
        }
    }
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
    private ChessPiece _piece;
    public ChessPiece Piece
    {
        get => _piece;
        set
        {
            if (_piece != value)
            {
                _piece = value;
                OnPropertyChanged(nameof(Piece));
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
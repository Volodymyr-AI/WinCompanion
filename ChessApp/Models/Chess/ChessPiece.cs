using System.Collections.ObjectModel;
using System.Diagnostics;
using ChessApp.Models.Board;

namespace ChessApp.Models.Chess;

public abstract class ChessPiece
{
    public PieceColor Color { get; set; }
    public abstract PieceType Type { get; }

    
    // Image path for binding
    public string ImagePath 
        => $"/WPF_WinCompanion;component/Images/PiecesImg/{Color}/{Type}.png";
    //WPF_WinCompanion/Images/PiecesImg
    
    public abstract bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board);

    public virtual ChessPiece Clone()
    {
        return (ChessPiece)MemberwiseClone();
    }
}
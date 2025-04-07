using System.Collections.ObjectModel;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;

namespace ChessApp.Models.Chess;

public abstract class ChessPiece
{
    public PieceColor Color { get; set; }
    public abstract PieceType Type { get; }

    
    // Image path for binding
    public string ImagePath 
        => $"/ChessApp;component/PiecesImg/{Color}/{Type}.png";
    
    public abstract bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board);
}
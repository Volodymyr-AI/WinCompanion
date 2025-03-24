using System.Collections.ObjectModel;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;

public abstract class ChessPiece
{
    public PieceColor Color { get; set; }
    public abstract PieceType Type { get; }

    
    // Image path for binding
    public string ImagePath 
        => $"/WPF_WinCompanion;component/Apps_Windows/Chess_App/ChessApp/{Color}/{Type}.png";
    
    public abstract bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board);
}
using System.Collections.ObjectModel;
using ChessApp.Models.Board;

namespace ChessApp.Models.Chess;

public abstract class ChessPiece
{
    public PieceColor Color { get; set; }
    public abstract PieceType Type { get; }

    
    // Image path for binding
    public string ImagePath 
        => $"/ChessApp;component/PiecesImg/{Color}/{Type}.png";
    //ChessApp/PiecesImg
    
    public abstract bool IsValidMove(ChessSquare from, ChessSquare to, ObservableCollection<ChessSquare> board);
}
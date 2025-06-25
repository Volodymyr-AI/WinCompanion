using ChessApp.Models.Chess;

namespace ChessApp.Services.GameHistory.Models;

/// <summary>
/// Simplified piece representation for snapshot
/// </summary>
public class PieceSnapshot
{
    public PieceType Type { get; set; }
    public PieceColor Color { get; set; }
    public bool HasMoved { get; set; } // special kings and rooks

    public PieceSnapshot(PieceType type, PieceColor color, bool hasMoved = false)
    {
        Type = type;
        Color = color;
        HasMoved = hasMoved;
    }
}
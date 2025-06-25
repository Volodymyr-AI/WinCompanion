namespace ChessApp.Services.GameHistory.Models;

/// <summary>
/// Snapshot of castling rights
/// </summary>
public class CastlingRightsSnapshot
{
    public bool WhiteKingMoved { get; set; }
    public bool BlackKingMoved { get; set; }
    public bool WhiteKingSideRookMoved { get; set; }
    public bool WhiteQueenSideRookMoved { get; set; }
    public bool BlackKingSideRookMoved { get; set; }
    public bool BlackQueenSideeRookMoved { get; set; }
}
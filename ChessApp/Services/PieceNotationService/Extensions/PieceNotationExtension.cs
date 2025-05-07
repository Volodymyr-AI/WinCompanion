using ChessApp.Models.Chess;
using ChessApp.Services.PieceNotationService.Utils;

namespace ChessApp.Services.PieceNotationService.Extensions;

public static class PieceNotationExtension
{
    public static string ToUnicode(this PieceShortcut pieceShortcut, bool isWhite)
    {
        return (pieceShortcut, isWhite) switch
        {
            (PieceShortcut.K, true) => "\u2654",
            (PieceShortcut.Q, true) => "\u2655",
            (PieceShortcut.R, true) => "\u2656",
            (PieceShortcut.B, true) => "\u2657",
            (PieceShortcut.N, true) => "\u2658",
            (PieceShortcut.P, true) => "\u2659",

            (PieceShortcut.K, false) => "\u265a",
            (PieceShortcut.Q, false) => "\u265b",
            (PieceShortcut.R, false) => "\u265c",
            (PieceShortcut.B, false) => "\u265d",
            (PieceShortcut.N, false) => "\u265e",
            (PieceShortcut.P, false) => "\u265f",

            _ => "?"
        };
    }
    
    public static string ToLetter(this PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.King => "K",
            PieceType.Queen => "Q",
            PieceType.Rook => "R",
            PieceType.Bishop => "B",
            PieceType.Knight => "N",
            PieceType.Pawn => "",
            _ => "?"
        };
    }
}
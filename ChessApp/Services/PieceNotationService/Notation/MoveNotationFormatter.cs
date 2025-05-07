using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.Services.PieceNotationService.Notation;

public class MoveNotationFormatter : IMoveNotationFormatter
{
    public string Format(Move move)
    {
        // 1) Castling
        if (move.IsKingSideCastle)  return "O-O";
        if (move.IsQueenSideCastle) return "O-O-O";

        var piece = move.NotationString;// "" for pawn, or "N","B","R","Q","K"
        var isPawn = string.IsNullOrEmpty(piece);

        // Capture sign
        var capture = move.IsCapture ? "x" : "";
        // check/mate suffix
        var suffix = move.IsCheckmate ? "#" :
            move.IsCheck     ? "+" : "";

        // 2) Pawn
        if (isPawn)
        {
            // if pawn attacks add column sign
            if (move.IsCapture)
            {
                var file = move.FromSquare[0]; // 'a'..'h'
                return $"{file}{capture}{move.ToSquare}{suffix}";
            }
            if (move.IsPawnPromotion)
            {
                return $"{move.ToSquare}=Q"; // або =N/R/B в залежності від вибору
            }
            // else just square and suffix
            return $"{move.ToSquare}{suffix}";
        }

        // 3) Other pieces: letter + (x?) + square + (+/#?)
        return $"{piece}{capture}{move.ToSquare}{suffix}";
    }
}
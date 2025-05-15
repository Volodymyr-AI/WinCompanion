using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation.Types;

public class SuffixProvider : ISuffixProvider
{
    public string GetSuffix(Move move)
    {
        if (move.IsCheckmate) return "#";
        if(move.IsCheck) return "+";
        return string.Empty;
    }
}
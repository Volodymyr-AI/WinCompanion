using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation.Types;

public class PawnNotationProvider : INotationPartProvider
{
    public bool IsApplicable(Move move) => string.IsNullOrEmpty(move.NotationString);

    public string Format(Move move)
    {
        var file = move.FromSquare[0];

        if (move.IsCapture)
            return $"{file}x{move.ToSquare}";
        if (move.IsPawnPromotion)
            return $"{move.ToSquare}=Q";
        
        return move.ToSquare;
    }
}
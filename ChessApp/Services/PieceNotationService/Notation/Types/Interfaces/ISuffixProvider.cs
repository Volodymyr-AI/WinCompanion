using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

public interface ISuffixProvider
{
    string GetSuffix(Move move);
}
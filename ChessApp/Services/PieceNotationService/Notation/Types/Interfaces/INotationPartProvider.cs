using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

public interface INotationPartProvider
{
    bool IsApplicable(Move move);
    string Format(Move move);
}
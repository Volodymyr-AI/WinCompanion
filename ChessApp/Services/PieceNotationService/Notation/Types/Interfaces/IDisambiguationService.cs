using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

public interface IDisambiguationService
{
    string Resolve(Move move);
}
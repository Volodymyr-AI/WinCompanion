using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation.Types;

public class DisambiguationService : IDisambiguationService
{
    /// <summary>
    /// Currently basic realisation
    /// For future: inject boardService and check all possible moves
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public string Resolve(Move move)
    {
        return string.Empty; // TODO: make a real logic
    }
}
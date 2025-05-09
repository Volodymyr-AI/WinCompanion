using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation.Types;

public class PieceNotationProvider : INotationPartProvider
{
    private readonly IDisambiguationService _disambiguationService;

    public PieceNotationProvider(IDisambiguationService disambiguationService)
    {
        _disambiguationService = disambiguationService;
    }

    public bool IsApplicable(Move move) => !string.IsNullOrEmpty(move.NotationString);

    public string Format(Move move)
    {
        var piece = move.NotationString;
        var disambigution = _disambiguationService.Resolve(move);
        var capture = move.IsCapture ? "x" : "";
        return $"{piece}{disambigution}{capture}{move.ToSquare}";
    }

}
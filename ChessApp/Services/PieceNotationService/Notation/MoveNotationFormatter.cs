using System.Numerics;
using ChessApp.Models.Board;
using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation;

public class MoveNotationFormatter : IMoveNotationFormatter
{
    private readonly List<INotationPartProvider> _providers;
    private readonly ISuffixProvider _suffixProvider;
    private readonly ChessBoardModel _board;

    public MoveNotationFormatter(ChessBoardModel board)
    {
        _board = board;
        _providers = new List<INotationPartProvider>()
        {
            new CastleNotationProvider(),
            new PawnNotationProvider(),
            new PieceNotationProvider(new DisambiguationService(_board))
        };
        _suffixProvider = new SuffixProvider();
    }

    public string Format(Move move)
    {
        foreach (var provider in _providers)
        {
            if (provider.IsApplicable(move))
            {
                return provider.Format(move) + _suffixProvider.GetSuffix(move);
            }
        }
        throw new InvalidOperationException("No applicable formatter found.");
    }
}
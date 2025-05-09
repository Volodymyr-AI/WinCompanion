using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation.Types;

public class CastleNotationProvider : INotationPartProvider
{
    public bool IsApplicable(Move move) => move.IsKingSideCastle || move.IsQueenSideCastle;

    public string Format(Move move) => move.IsKingSideCastle ? "O-O" : "O-O-O";
}
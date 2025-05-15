using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.Services.PieceNotationService.Notation;

public interface IMoveNotationFormatter
{
    string Format(Move move);
}
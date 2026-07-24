using ChessApp.BoardLogic.Game.Validators;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.EnPassantValidation;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Actions.Highlight;

public interface IMoveHighlighter
{
    void HighlightMoves(ChessSquare selectedSquare, ChessBoardModel board, CastlingValidator castlingValidator, EnPassantValidator enPassantValidator);
    void ClearHighlights(ChessBoardModel board);
}
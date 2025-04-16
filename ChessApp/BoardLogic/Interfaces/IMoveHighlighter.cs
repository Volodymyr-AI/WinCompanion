using ChessApp.BoardLogic.Game.Validators;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Interfaces;

public interface IMoveHighlighter
{
    void HighlightMoves(ChessSquare selectedSquare, ChessBoardModel board, CastlingValidator castlingValidator);
    void ClearHighlights(ChessBoardModel board);
}
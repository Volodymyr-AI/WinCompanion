using System.Windows.Media;
using ChessApp.BoardLogic.Game.Generators;
using ChessApp.BoardLogic.Game.Validators;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.Models.Board;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Actions.Highlight;

public class MoveHighlight : IMoveHighlighter
{
    /// <summary>
    /// Highlight selected pieces possible moves
    /// </summary>
    public void HighlightMoves(ChessSquare selectedSquare, ChessBoardModel board, CastlingValidator castlingValidator)
    {
        List<ChessSquare> possibleMoves = selectedSquare.Piece is King
            ? MoveGenerator.GetKingMoves(selectedSquare, board, castlingValidator)
            : MoveGenerator.GetPossibleMoves(selectedSquare, board);

        foreach (var square in possibleMoves)
        {
            if (selectedSquare.Piece is King &&
                CheckMateValidator.IsKingCheckAfterMove(board, selectedSquare, square))
            {
                square.Background = square.BaseBackground;
            }
            else if (square.Piece != null)
            {
                square.Background = Brushes.LightCoral;
            }
            else
            {
                square.Background = Brushes.LightBlue;
            }
        }
    }

    /// <summary>
    /// Clears the highlighting of all squares by resetting 
    /// their Background property to the BaseBackground.
    /// </summary>
    public void ClearHighlights(ChessBoardModel board)
    {
        foreach (var square in board.Squares)
        {
            square.Background = square.BaseBackground;
        }
    }
}
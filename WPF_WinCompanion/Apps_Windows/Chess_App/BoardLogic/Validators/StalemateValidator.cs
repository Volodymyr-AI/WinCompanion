using System.Diagnostics;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;

public class StalemateValidator
{
    public static bool IsStalemate(ChessBoardModel board, PieceColor player)
    {
        ChessSquare king = board.Squares.First(sq => sq.Piece is King && sq.Piece.Color == player);
        if (king is null)
        {
            Debug.WriteLine("IsStalemate: King is null");
            return false;
        }

        // If King is under attack ( check ) it's not a stalemate
        if (CheckMateValidator.IsKingCheck(board, player))
        {
            Debug.WriteLine("IsStalemate: King is in check");
            return false;
        }

        foreach (var square in board.Squares)
        {
            if (square.Piece != null && square.Piece.Color == player)
            {
                var possibleMoves = MoveGenerator.GetPossibleMoves(square, board)
                    .Where(target => !CheckMateValidator.IsKingCheckAfterMove(board, square, target)) // filter dangerous moves
                    .ToList();

                if (possibleMoves.Any())
                {
                    Debug.WriteLine($"IsStalemate: {square.Piece.GetType().Name} at {square.Row}, {square.Column} has valid moves.");
                    return false;
                }
            }
        }
        
        Debug.WriteLine("IsStalemate: No legal moves available. Stalemate!");
        return true;
    }
    
}
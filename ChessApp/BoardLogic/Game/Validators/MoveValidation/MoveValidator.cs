using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Validators.MoveValidation;

public sealed class MoveValidator : IMoveValidator
{
    public bool IsMoveValid(
        ChessBoardModel board,
        ChessSquare from, 
        ChessSquare to, 
        PieceColor turn, 
        out string error)
    {
        error = string.Empty;
        
        if (from.Piece == null || from.Piece.Color != turn)
        {
            error = "No piece to move or wrong turn.";
            return false;
        }
        if (!from.Piece.IsValidMove(from, to, board.Squares))
        {
            error = "Invalid move for this piece.";
            return false;
        }
        
        if (CheckMateValidator.IsKingCheck(board, turn))
        {
            if (from.Piece is King king &&
                !CheckMateValidator.IsSafeForKingToMove(board, from, to))
            {
                error = "King is still in check.";
                return false;
            }

            if (from.Piece is not King &&
                !CheckMateValidator.DoesMoveDefendKing(board, from, to))
            {
                error = "This move doesn't remove the check.";
                return false;
            }
        }
        else if (CheckMateValidator.DoesMoveExposeKingToCheck(board, from, to))
        {
            error = "This move exposes the King to check.";
            return false;
        }

        return true;
    }
}
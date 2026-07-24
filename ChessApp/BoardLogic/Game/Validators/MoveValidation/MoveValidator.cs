using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.BoardLogic.Game.Validators.EnPassantValidation;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Validators.MoveValidation;

public sealed class MoveValidator : IMoveValidator
{
    private readonly EnPassantValidator _enPassantValidator;

    public MoveValidator(EnPassantValidator enPassantValidator)
    {
        _enPassantValidator = enPassantValidator;
    }

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

        // En passant target square is empty, so Pawn.IsValidMove (which requires a piece
        // to capture diagonally) rejects it - special-case it here instead.
        bool isEnPassant = from.Piece is Pawn && _enPassantValidator.IsEnPassantCapture(from, to, board);
        if (!isEnPassant && !from.Piece.IsValidMove(from, to, board.Squares))
        {
            error = "Invalid move for this piece.";
            return false;
        }

        // An en passant capture removes a pawn from a square other than "to", which can expose
        // the king to a discovered check (e.g. along the rank the captured pawn used to block).
        // Temporarily lift it off the board so the check simulations below see the real result.
        ChessSquare? enPassantCapturedSquare = isEnPassant
            ? _enPassantValidator.GetEnPassantCapturedPawnSquare(to, from.Piece.Color, board)
            : null;
        ChessPiece? enPassantCapturedPiece = enPassantCapturedSquare?.Piece;
        if (enPassantCapturedSquare != null)
        {
            enPassantCapturedSquare.Piece = null;
        }

        try
        {
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
        finally
        {
            if (enPassantCapturedSquare != null)
            {
                enPassantCapturedSquare.Piece = enPassantCapturedPiece;
            }
        }
    }
}
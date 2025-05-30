﻿using ChessApp.BoardLogic.Game.Generators;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Validators.StalemateValidation;

public class StalemateValidator
{
    public static bool IsStalemate(ChessBoardModel board, PieceColor player)
    {
        ChessSquare king = board.Squares.First(sq => sq.Piece is King && sq.Piece.Color == player);
        if (king is null)
        {
            return false;
        }

        // If King is under attack ( check ) it's not a stalemate
        if (CheckMateValidator.IsKingCheck(board, player))
        {
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
                    return false;
                }
            }
        }
        
        return true;
    }
    
}
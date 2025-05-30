﻿using System.Diagnostics;
using ChessApp.BoardLogic.Game.Generators;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Validators.CheckmateValidation;

public static class CheckMateValidator
{
    /// <summary>
    ///  Check if King is under attack
    /// </summary>
    /// <param name="board"></param>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    public static bool IsKingCheck(ChessBoardModel board, PieceColor currentTurn)
    {
        ChessSquare kingSquare = board.Squares.FirstOrDefault(s => s.Piece is King && s.Piece.Color == currentTurn);
        if (kingSquare == null) return false;

        foreach (var square in board.Squares)
        {
            if (square.Piece == null || square.Piece.Color == currentTurn)
                continue; // Skip ally pieces

            List<ChessSquare> possibleMoves;

            if (square.Piece is Pawn)
            {
                possibleMoves = MoveGenerator.GetPawnAttackSquare(square, board); // Pawns have a unique attack, they cannot attack the way they move, only by diagonal so we need to check this moment
            }
            else
            {
                possibleMoves = MoveGenerator.GetPossibleMoves(square, board); // For all other pieces
            }

            if (possibleMoves.Contains(kingSquare))
            {
                Debug.WriteLine($"Check detected! {square.Piece.GetType().Name} at {square.Row}, {square.Column} can attack the king!");
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if King is under checkmate
    /// </summary>
    /// <param name="board"></param>
    /// <param name="kingColor"></param>
    /// <returns></returns>
    public static bool IsCheckmate(ChessBoardModel board, PieceColor kingColor)
    {
        if (!IsKingCheck(board, kingColor))
        {
            return false;
        }
        
        ChessSquare kingSquare = board.Squares.First(s => s.Piece?.Type == PieceType.King && s.Piece.Color == kingColor);
        var possibleKingMoves = MoveGenerator.GetPossibleMoves(kingSquare, board)
            .Where(move => !IsKingCheckAfterMove(board, kingSquare, move))
            .ToList();
        
        foreach (var move in possibleKingMoves)
        {
            Debug.WriteLine($"Possible king escape: {ToChessNotation(move.Row, move.Column)}");
        }

        if (possibleKingMoves.Any())
            return false;
        
        // Check if other allie pieces can protect the King
        if(CanDefendKing(board, kingColor))
            return false;

        return true; // Mate if no possible moves
    }

    /// <summary>
    /// Simulation of possible King moves to know if it still is under check after move
    /// </summary>
    /// <param name="board"></param>
    /// <param name="from">Current King square</param>
    /// <param name="to">Square where King can make a move</param>
    /// <returns>Returns true for the move if King still under attack and false if not</returns>
    public static bool IsKingCheckAfterMove(ChessBoardModel board, ChessSquare from, ChessSquare to)
    {
        if (board == null || from == null || to == null || from.Piece == null)
            return false;
        
        var backupPiece = to.Piece; // Save the piece that is in a desirable square
        var movedPiece = from.Piece; // Save the piece we are moving (King)
        
        to.Piece = movedPiece; // Our test move to simulate 
        from.Piece = null; // And making null square where piece used to be
        
        bool inCheck = IsKingCheck(board, movedPiece.Color); // Now we check if King under check after out virtual move
        
        // And now rolling back not to influence a game
        from.Piece = movedPiece; 
        to.Piece = backupPiece;
        
        return inCheck;
    }
    /// <summary>
    /// Examine if any piece can help King avoid check by 2 ways
    /// 1) Take a piece that attacks King
    /// 2) Protect King by standing above him
    /// </summary>
    /// <param name="board"></param>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    public static bool CanDefendKing(ChessBoardModel board, PieceColor kingColor)
    {
        if(!IsKingCheck(board, kingColor)) return false;
        // Find square of a King
        ChessSquare kingSquare = board.Squares.First(sq => sq.Piece is King && sq.Piece.Color == kingColor);

        // Find all opponent pieces that can attack King
        List<ChessSquare> attackingPieces = board.Squares
            .Where(sq => sq.Piece != null
                         && sq.Piece.Color != kingColor
                         && sq.Piece.IsValidMove(sq, kingSquare, board.Squares))
            .ToList();
        //if (attackingPieces.Any()) return false; // no checkmate if there is no attacking squares

        // If King is under attack from 2 pieces at a time only moving King will help
        if (attackingPieces.Count > 1)
        {
            return false; // Only King move helps
        }
        
        ChessSquare attackerSquare = attackingPieces.First();
        // Check if we can take an attacking piece
        bool canCaptureAttacker = board.Squares
            .Any(sq => sq.Piece != null
                       && sq.Piece.Color == kingColor
                       && sq.Piece.IsValidMove(sq, attackerSquare, board.Squares)
                       && !IsKingCheckAfterMove(board, sq, attackerSquare));

        if (canCaptureAttacker)
            return true;

        // Verify if there is a way to defend from check ( not working against Knight )
        if (!(attackerSquare.Piece is Knight))
        {
            List<ChessSquare> blockingSquare = GetBlockingSquare(board, kingSquare, attackerSquare);
            foreach (var blockSquare in blockingSquare)
            {
                bool canBlock = board.Squares
                    .Any(sq => sq.Piece != null
                               && sq.Piece.Color == kingColor
                               && sq.Piece.IsValidMove(sq, blockSquare, board.Squares)
                               && !IsKingCheckAfterMove(board, blockSquare, blockSquare));
                
                if(canBlock)
                    return true;
            }

            if (!canCaptureAttacker && blockingSquare.Count == 0)
            {
                return false; // No defense => checkmate
            }
        }
        return false; // No way to protect King
    }

    /// <summary>
    /// If check is made by Rook, Bishop or Queen find a square between King and attacking piece for "protection"
    /// </summary>
    /// <param name="board"></param>
    /// <param name="kingSquare"></param>
    /// <param name="attackerSquare"></param>
    /// <returns></returns>
    private static List<ChessSquare> GetBlockingSquare(ChessBoardModel board, ChessSquare kingSquare, ChessSquare attackerSquare)
    {
        List<ChessSquare> blockingSquares = new List<ChessSquare>();
        
        int rowDiraction = Math.Sign(attackerSquare.Row - kingSquare.Row);
        int colDiraction = Math.Sign(attackerSquare.Column - kingSquare.Column);
        
        int row = kingSquare.Row + rowDiraction;
        int col = kingSquare.Column + colDiraction;

        while (row != attackerSquare.Row || col != attackerSquare.Column)
        {
            blockingSquares.Add(board.Squares.First(sq => sq.Row == row && sq.Column == col));
            row += rowDiraction;
            col += colDiraction;
        }
        return blockingSquares;
    }
    
    /// <summary>
    /// Checks if moving a piece would expose the King to check.
    /// </summary>
    public static bool DoesMoveExposeKingToCheck(ChessBoardModel board, ChessSquare selectedSquare, ChessSquare destinationSquare)
    {
        // Simulate move
        ChessPiece tempPiece = destinationSquare.Piece;
        destinationSquare.Piece = selectedSquare.Piece;
        selectedSquare.Piece = null;

        bool exposesKing = IsKingCheck(board, destinationSquare.Piece.Color);

        // Cancel simulation
        selectedSquare.Piece = destinationSquare.Piece;
        destinationSquare.Piece = tempPiece;

        return exposesKing;
    }
    
    /// <summary>
    /// Check if King won't be under check after the move
    /// </summary>
    public static bool IsSafeForKingToMove(ChessBoardModel board, ChessSquare selectedSquare, ChessSquare destinationSquare)
    {
        return !(selectedSquare.Piece is King && IsKingCheckAfterMove(board, selectedSquare, destinationSquare));
    }
    
    /// <summary>
    /// Simulate a piece move to check if it can defend an ally King
    /// </summary>
    public static bool DoesMoveDefendKing(ChessBoardModel board, ChessSquare from, ChessSquare to)
    {
        ChessPiece tempPiece = to.Piece;
        to.Piece = from.Piece;
        from.Piece = null;
        
        bool stillInCheck = IsKingCheck(board, to.Piece.Color);
        
        from.Piece = to.Piece;
        to.Piece = tempPiece;

        return !stillInCheck;
    }


    // Debugging method
    private static string ToChessNotation(int row, int col)
    {
        char file = (char)('A' + col);
        int rank = 8 - row;
        return $"{file}{rank}";
    }
}
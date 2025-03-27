using System.Diagnostics;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;

public static class CheckMateValidator
{
    /// <summary>
    ///  
    /// </summary>
    /// <param name="board"></param>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    public static bool IsKingCheck(ChessBoardModel board, PieceColor currentTurn)
    {
        ChessSquare kingSquare = board.Squares.FirstOrDefault(s => s.Piece is King && s.Piece.Color == currentTurn);
        if (kingSquare == null) return false;
        Debug.WriteLine(kingSquare != null ? $"King found at {kingSquare.Row}, {kingSquare.Column}" : "King not found");
        
        foreach (var square in board.Squares)
        {
            var possibleMoves = MoveGenerator.GetPossibleMoves(square, board);
            if (possibleMoves.Contains(kingSquare))
            {
                Debug.WriteLine($"Check detected! {square.Piece.GetType().Name} at {square.Row}, {square.Column} can attack the king!");
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// 
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
        Debug.WriteLine($"{kingSquare} found; Class: CheckMateValidator");
        var possibleKingMoves = MoveGenerator.GetPossibleMoves(kingSquare, board)
            .Where(move => !IsKingCheckAfterMove(board, kingSquare, move))
            .ToList();
        
        foreach (var move in possibleKingMoves)
        {
            Debug.WriteLine($"Possible king escape: {move.Row}, {move.Column}");
        }

        if (possibleKingMoves.Any())
            return false;
        
        // Check if other allie pieces can protect the King
        if(CanDefendKing(board, kingColor))
            return false;

        return true; // Mate if no possible moves
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="board"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static bool IsKingCheckAfterMove(ChessBoardModel board, ChessSquare from, ChessSquare to)
    {
        if (board == null || from == null || to == null || from.Piece == null)
            return false;
        
        var bakcupPiece = to.Piece;
        var movedPiece = from.Piece;
        
        to.Piece = movedPiece;
        from.Piece = null;
        
        bool inCheck = IsKingCheck(board, movedPiece.Color);
        
        from.Piece = movedPiece;
        to.Piece = bakcupPiece;
        
        Debug.WriteLine($"Checking if king is in check after move: {inCheck}");
        return inCheck;
    }
    
    public static bool CanDefendKing(ChessBoardModel board, PieceColor currentTurn)
    {
        // Find square of a King
        ChessSquare kingSquare = board.Squares.FirstOrDefault(sq => sq.Piece is King && sq.Piece.Color == currentTurn);
        
        if (kingSquare == null) return false;

        // Find all opponent pieces that can attack King
        List<ChessSquare> attackingPieces = board.Squares
            .Where(sq => sq.Piece != null
                         && sq.Piece.Color != currentTurn
                         && sq.Piece.IsValidMove(sq, kingSquare, board.Squares))
            .ToList();

        // If King is under attack from 2 pieces at a time only moving King will help
        if (attackingPieces.Count > 1)
        {
            return false; // Only King move helps
        }
        
        
        ChessSquare attackerSquare = attackingPieces.FirstOrDefault();
        if (attackerSquare == null)
        {
            return false; // If no pieces trying to attack the King 
        }
        // Check if we can take an attacking piece
        bool canCaptureAttacker = board.Squares
            .Any(sq => sq.Piece != null
                       && sq.Piece.Color == currentTurn
                       && sq.Piece.IsValidMove(sq, attackerSquare, board.Squares));

        if (canCaptureAttacker)
        {
            return true;
        }

        // Check if there is a way to defend from check ( not working against Knight )
        if (!(attackerSquare.Piece is Knight))
        {
            List<ChessSquare> blockingSquare = GetBlockingSquare(board, kingSquare, attackerSquare);
            foreach (var blockSquare in blockingSquare)
            {
                bool canBlock = board.Squares
                    .Any(sq => sq.Piece != null
                               && sq.Piece.Color == currentTurn
                               && sq.Piece.IsValidMove(sq, blockSquare, board.Squares));
                
                if(canBlock)
                    return true;
            }

            if (!canCaptureAttacker && blockingSquare.Count == 0)
            {
                Debug.WriteLine("Checkmate Debug: No way to defend the king!");
                return false; // No defense => checkmate
            }
        }
        Debug.WriteLine("Checkmate Debug: King is in check, but CanDefendKing returned true.");
        return false; // No way to protect King
    }

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
}
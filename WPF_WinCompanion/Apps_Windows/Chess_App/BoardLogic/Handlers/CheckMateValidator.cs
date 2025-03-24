using System.Diagnostics;
using Microsoft.Xaml.Behaviors.Core;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class CheckMateValidator
{
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

        if (possibleKingMoves.Any())
            return false;

        return true; // Mate if no possible moves
    }

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
        
        return inCheck;
    }
}
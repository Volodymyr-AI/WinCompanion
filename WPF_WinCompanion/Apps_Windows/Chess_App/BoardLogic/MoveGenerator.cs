using System.Windows.Automation.Text;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic;

public static class MoveGenerator
{
    public static List<ChessSquare> GetPossibleMoves(ChessSquare square, ChessBoardModel board)
    {
        if (square.Piece == null)
        {
            return new List<ChessSquare>();
        }

        switch (square.Piece.Type)
        {
            case PieceType.Pawn:
                return GetPawnMoves(square, board);
            case PieceType.Knight:
                return GetKnightMoves(square, board);
            case PieceType.Bishop:
                return GetBishopMoves(square, board);
            case PieceType.Rook:
                return GetRookMoves(square, board);
            case PieceType.Queen:
                return GetQueenMoves(square, board);
            case PieceType.King:
                return GetKingMoves(square, board);
            default:
                return new List<ChessSquare>();
        }
    }

    private static List<ChessSquare> GetPawnMoves(ChessSquare pawn, ChessBoardModel board)
    {
        var moves = new List<ChessSquare>();
        int direction = (pawn.Piece.Color == PieceColor.White) ? 1 : -1;
        int startRow = (pawn.Piece.Color == PieceColor.White) ? 6 : 1;
        
        // One move forward
        ChessSquare? forwardSquare = board.GetSquare(pawn.Row + direction, pawn.Column);
        if (forwardSquare != null && forwardSquare.Piece == null)
        {
            moves.Add(forwardSquare);
        }
        
        // Two moves forward if first move
        if (pawn.Row == startRow)
        {
            ChessSquare? doubleMove = board.GetSquare(pawn.Row + direction * 2, pawn.Column);
            if (doubleMove != null && doubleMove.Piece == null)
            {
                moves.Add(doubleMove);
            }
        }

        // Diagonal attack
        int[] attackColumns = { -1, 1 };
        foreach (int side in attackColumns)
        {
           ChessSquare? diagonal = board.GetSquare(pawn.Row + direction, pawn.Column + side);
           if (diagonal != null && diagonal.Piece != null && diagonal.Piece.Color != pawn.Piece.Color)
           {
               moves.Add(diagonal);
           }
        }
        return moves;
    }
    
    /// <summary>
    /// Get squares pawn can attack
    /// </summary>
    public static List<ChessSquare> GetPawnAttackSquare(ChessSquare pawn, ChessBoardModel board)
    {
        var attackSquares = new List<ChessSquare>();
        int direction = (pawn.Piece.Color == PieceColor.White) ? -1 : 1;

        int[] attackColumns = { -1, 1 };
        foreach (int side in attackColumns)
        {
            ChessSquare? diagonal = board.GetSquare(pawn.Row + direction, pawn.Column + side);
            if (diagonal != null)
            {
                attackSquares.Add(diagonal);
            }
        }

        return attackSquares;
    }

    private static List<ChessSquare> GetKnightMoves(ChessSquare knight, ChessBoardModel board)
    {
        var moves = new List<ChessSquare>();
        int[][] directions =
        [
            [-2, -1], [-2, 1], [-1, -2], [-1, 2],
            [1, -2], [1, 2], [2, -1], [2, 1]
        ];

        foreach (var direction in directions)
        {
            ChessSquare? targetSquare = board.GetSquare(knight.Row + direction[0], knight.Column + direction[1]);
            if(targetSquare != null && (targetSquare.Piece == null || targetSquare.Piece.Color != knight.Piece.Color))
                moves.Add(targetSquare);
        }
        
        return moves;
    }

    private static List<ChessSquare> GetRookMoves(ChessSquare rook, ChessBoardModel board)
    {
        return LinearMoves(rook, board, [[-1, 0], [1, 0], [0, -1], [0, 1]]);
    }

    private static List<ChessSquare> GetBishopMoves(ChessSquare bishop, ChessBoardModel board)
    {
        return LinearMoves( bishop, board, [[-1, -1], [-1, 1], [1, -1], [1, 1]]);
    }
    private static List<ChessSquare> GetQueenMoves(ChessSquare queen, ChessBoardModel board)
    {
        return LinearMoves(queen, board, [
            [-1, -1], [-1, 1], [1, -1], [1, 1],
            [-1, 0], [1, 0], [0, -1], [0, 1]
        ]);
    }

    private static List<ChessSquare> GetKingMoves(ChessSquare king, ChessBoardModel board)
    {
        var moves = new List<ChessSquare>();
        int[][] directions =
        [
            [-1, -1],[-1, 0],[-1, 1],
            [0, -1],       [0, 1],
            [1, -1],[1, 0],[1, 1]
        ];
        foreach (var direction in directions)
        {
            ChessSquare? targetSquare = board.GetSquare(king.Row + direction[0], king.Column + direction[1]);
            if (targetSquare != null && (targetSquare.Piece == null || targetSquare.Piece.Color != king.Piece.Color))
            {
                moves.Add(targetSquare);
            }
        }
        return moves;
    }

    private static List<ChessSquare> LinearMoves(ChessSquare figure, ChessBoardModel board, int[][] directions)
    {
        var moves = new List<ChessSquare>();

        foreach (var direction in directions)
        {
            int row = figure.Row + direction[0];
            int column = figure.Column + direction[1];

            while (board.IsInsideBoard(row, column))
            {
                ChessSquare? targetSquare = board.GetSquare(row, column);

                if (targetSquare.Piece == null)
                {
                    moves.Add(targetSquare);
                }
                else
                {
                    if (targetSquare.Piece.Color != figure.Piece.Color)
                        moves.Add(targetSquare);
                    break;
                }

                row += direction[0];
                column += direction[1];
            }
        }
        
        return moves;
    }
}
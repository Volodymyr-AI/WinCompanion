using System.Collections.ObjectModel;
using System.Windows;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class CastlingValidator
{
    private readonly Dictionary<PieceColor, bool> _kingMoved = new()
    {
        { PieceColor.White, false },
        { PieceColor.Black, false }
    };

    private readonly Dictionary<(PieceColor, int), bool> _rookMoved = new()
    {
        { (PieceColor.White, 0), false },
        { (PieceColor.White, 7), false },
        { (PieceColor.Black, 0), false },
        { (PieceColor.Black, 7), false },
    };

    public bool CanCastle(ChessSquare kingSquare, ChessSquare rookSquare, ChessBoardModel board)
    {
        if (_kingMoved[kingSquare.Piece.Color] || _rookMoved[(kingSquare.Piece.Color, rookSquare.Column)])
            return false;
        if(kingSquare.Row != rookSquare.Row)
            return false;

        int step = rookSquare.Column > kingSquare.Column ? 1 : -1;
        
        //Check if between King and Rook no either pieces
        for (int col = kingSquare.Column + step; col != rookSquare.Column; col += step)
        {
            if (board.Squares.Any(sq => sq.Row == kingSquare.Row && sq.Column == col && sq.Piece != null))
            {
                return false;
            }
        }
        
        // Check if the King not under check
        for (int col = kingSquare.Column; col != kingSquare.Column + 2 * step; col += step)
        {
            var kingSquareMoveTo = board.Squares.First(sq => sq.Row == kingSquare.Row && sq.Column == col);
            if (CheckMateValidator.IsKingCheckAfterMove(board, kingSquare, kingSquareMoveTo))
            {
                return false;
            }
        }

        return true;
    }

    public void MarkKingMoved(PieceColor pieceColor)
    {
        _kingMoved[pieceColor] = true;
    }

    public void MarkRookMoved(PieceColor pieceColor, int column)
    {
        _rookMoved[(pieceColor, column)] = true;
    }
}
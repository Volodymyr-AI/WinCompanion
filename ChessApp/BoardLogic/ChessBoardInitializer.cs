using ChessApp.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic;

public static class ChessBoardInitializer
{
    public static void InitializeBoard(ChessBoardModel boardModal)
    {
        boardModal.Squares.Clear();

        foreach (var square in BoardGenerator.GenerateSquares())
        {
            square.Piece = GetInitialPiece(square.Row, square.Column);
            boardModal.Squares.Add(square);
        }
    }

    private static ChessPiece? GetInitialPiece(int row, int col)
    {
        if(col < 0 || col > 7) return null;

        return row switch
        {
            1 => new Pawn { Color = PieceColor.Black },
            6 => new Pawn { Color = PieceColor.White },
            0 or 7 => GetPieceTypeForColumn(row, col),
            _ => null
        };
    }

    private static ChessPiece GetPieceTypeForColumn(int row, int col)
    {
        var color = (row == 0) ? PieceColor.Black : PieceColor.White;

        return col switch
        {
            0 or 7 => new Rook { Color = color },
            1 or 6 => new Knight { Color = color },
            2 or 5 => new Bishop { Color = color },
            3 => new Queen { Color = color },
            4 => new King { Color = color },
            _ => throw new InvalidOperationException("Invalid column for piece"),
        };
    }
}
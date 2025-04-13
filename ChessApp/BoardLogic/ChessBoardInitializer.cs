using System;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;


namespace ChessApp.BoardLogic;

public static class ChessBoardInitializer
{
    public static void InitializeBoard(ChessBoardModel boardModel)
    {
        boardModel.Squares.Clear();

        foreach (var square in BoardGenerator.GenerateSquares())
        {
            square.Piece = GetInitialPiece(square.Row, square.Column);
            boardModel.Squares.Add(square);
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
    
    // Method for testing
    public static ChessBoardModel InitializeEmptyBoard()
    {
        var model = new ChessBoardModel();
        
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                model.Squares.Add(new ChessSquare
                {
                    Row = row,
                    Column = col,
                    Piece = null
                });
            }
        }

        return model;
    }
}
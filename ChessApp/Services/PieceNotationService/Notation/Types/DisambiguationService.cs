using System.Security.Cryptography;
using ChessApp.BoardLogic.Game.Generators;
using ChessApp.Models.Board;
using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation.Types.Interfaces;

namespace ChessApp.Services.PieceNotationService.Notation.Types;

public class DisambiguationService : IDisambiguationService
{
    private ChessBoardModel _board;

    public DisambiguationService(ChessBoardModel board)
    {
        _board = board;
    }
    
    
    
    /// <summary>
    /// Currently basic realisation
    /// For future: inject boardService and check all possible moves
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public string Resolve(Move move)
    {
        if(string.IsNullOrEmpty(move.NotationString) || move.IsKingSideCastle || move.IsQueenSideCastle)
            return string.Empty;
        
        int toRow = 8 - int.Parse(move.ToSquare[1].ToString());
        int toCol = move.ToSquare[0] - 'a';
        
        // Get a piece that can move
        var fromSquare = GetSquareFromNotation(move.FromSquare);
        
        // Get all pieces same type and color, that can make a same move ( to the same square )
        var ambiguousPieces = FindAmbiguousPieces(fromSquare, toRow, toCol);
        
        // If there is any, ambiguous does not needed
        if(ambiguousPieces.Count <= 1)
            return string.Empty;

        // Checking, if we need to specify file (column)
        var piecesInDifferentFiles = ambiguousPieces.GroupBy(p => p.Column).Count() > 1;
        if (piecesInDifferentFiles)
        {
            return ((char)('a' + fromSquare.Column)).ToString();
        }
        
        // If files(columns) are the same, but rows different, specify row
        var piecesInDifferentRanks = ambiguousPieces.GroupBy(p => p.Row).Count() > 1;
        if (piecesInDifferentRanks)
        {
            return ( 8 - fromSquare.Row).ToString();
        }

        // If files(columns) and ranks(rows) are the same then we ambigue
        return $"{(char)('a' + fromSquare.Column)}{8 - fromSquare.Row}";
    }

    private ChessSquare GetSquareFromNotation(string notation)
    {
        int col = notation[0] - 'a';
        int row = 8 - int.Parse(notation[1].ToString());
        return _board.GetSquare(col, row);
    }

    private List<ChessSquare> FindAmbiguousPieces(ChessSquare fromSquare, int toRow, int toCol)
    {
        var toSquare = _board.GetSquare(toRow, toCol);
        if (toSquare == null || fromSquare.Piece == null)
        {
            return new List<ChessSquare>();
        }

        var pieceType = fromSquare.Piece.Type;
        var pieceColor = fromSquare.Piece.Color;
        
        return _board.Squares
            .Where(s => s.Piece != null
                && s.Piece.Type == pieceType
                && s.Piece.Color == pieceColor
                && !(s.Row == fromSquare.Row && s.Column == fromSquare.Column)
                && CanMoveToTarget(s, toRow, toCol))
            .ToList();
    }

    private bool CanMoveToTarget(ChessSquare square, int toRow, int toCol)
    {
        var targetSquare = _board.GetSquare(toRow, toCol);
        if(targetSquare == null ||  square.Piece == null)
            return false;

        var possibleMoves = MoveGenerator.GetPossibleMoves(square, _board);
        return possibleMoves.Contains(targetSquare);
    }
}
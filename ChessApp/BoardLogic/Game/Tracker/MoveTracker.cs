using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Extensions;

namespace ChessApp.BoardLogic.Game.Tracker;

public class MoveTracker
{
    private readonly ChessBoardModel _board;
    private ChessSquare _lastFromSquare;
    private ChessSquare _lastToSquare;
    private ChessPiece _lastMovedPiece;
    private ChessPiece _capturedPiece;
    private bool _wasKingSideCastle;
    private bool _wasQueenSideCastle;
    private bool _wasPromotion;
    
    public MoveTracker(ChessBoardModel board)
    {
        _board = board;
    }
    
    
    public void RecordMoveStart(ChessSquare fromSquare)
    {
        _lastFromSquare = fromSquare;
        _lastMovedPiece = fromSquare.Piece?.Clone() as ChessPiece;
    }
    
    public void RecordMoveEnd(ChessSquare toSquare, bool isKingSideCastle = false, bool isQueenSideCastle = false, bool isPawnPromotion = false)
    {
        _lastToSquare = toSquare;
        _wasKingSideCastle = isKingSideCastle;
        _wasQueenSideCastle = isQueenSideCastle;
        _wasPromotion = isPawnPromotion;
    }
    
    public void RecordCapture(ChessPiece capturedPiece)
    {
        _capturedPiece = capturedPiece;
    }
    
    public Move CreateMove(int moveNumber, PieceColor color, bool isCheck = false, bool isCheckmate = false)
    {
        if (_lastFromSquare == null || _lastToSquare == null || _lastMovedPiece == null)
            throw new InvalidOperationException("Move information is incomplete");
            
        string fromNotation = SquareToAlgebraic(_lastFromSquare.Row, _lastFromSquare.Column);
        string toNotation = SquareToAlgebraic(_lastToSquare.Row, _lastToSquare.Column);
        string pieceNotation = _lastMovedPiece.Type.ToLetter();
        
        var move = new Move
        {
            Id = 0, 
            GameId = 0, 
            MoveNumber = moveNumber,
            Color = color.ToString(),
            NotationString = pieceNotation,
            FromSquare = fromNotation,
            ToSquare = toNotation,
            IsCapture = _capturedPiece != null,
            IsCheck = isCheck,
            IsCheckmate = isCheckmate,
            IsKingSideCastle = _wasKingSideCastle,
            IsQueenSideCastle = _wasQueenSideCastle,
            IsPawnPromotion = _wasPromotion
        };
        
        ResetState();
        
        return move;
    }
    
    private void ResetState()
    {
        _lastFromSquare = null;
        _lastToSquare = null;
        _lastMovedPiece = null;
        _capturedPiece = null;
        _wasKingSideCastle = false;
        _wasQueenSideCastle = false;
        _wasPromotion = false;
    }
    
    private string SquareToAlgebraic(int row, int col)
    {
        char file = (char)('a' + col);
        int rank = 8 - row;
        return $"{file}{rank}";
    }
}
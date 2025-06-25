using System.Windows.Media;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;
using ChessApp.Services.GameHistory.Models;

namespace ChessApp.Services.GameHistory;

public class GameHistoryManager
{
    private readonly List<BoardStateSnapshot> _gameHistory = new();
    private int _currentMoveIndex = -1; // -1 means current live game state
    private BoardStateSnapshot? _liveGameState; // Store current live game state

    public bool IsViewingHistory => _currentMoveIndex >= 0;
    public bool CanNavigateBack => _gameHistory.Count > 0;
    public bool CanNavigateForward => _currentMoveIndex >= 0 && _currentMoveIndex < _gameHistory.Count - 1;
    public int CurrentMoveIndex => _currentMoveIndex;
    public int TotalMoves => _gameHistory.Count;

    public void CaptureCurrentState(
        ChessBoardModel board,
        PieceColor currentTurn,
        int halfMoveCounter,
        CastlingValidator castlingValidator,
        int moveNumber)
    {
        var boardState = new PieceSnapshot?[8, 8];
        
        // Capture current board state
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = board.GetSquare(row, col);
                if (square?.Piece != null)
                {
                    bool hasMoved = square.Piece switch
                    {
                        King king => king.HasMoved,
                        Rook rook => rook.HasMoved,
                        _ => false
                    };
                    boardState[row, col] = new PieceSnapshot(
                        square.Piece.Type,
                        square.Piece.Color,
                        hasMoved);
                }
            }
        }
        
        // Capture castling rights
        var castlingRights = CaptureCastlingRights(castlingValidator);
        
        var snapshot = new BoardStateSnapshot(
            moveNumber,
            currentTurn,
            halfMoveCounter,
            boardState,
            castlingRights);

        if (_currentMoveIndex >= 0)
        {
            _gameHistory.RemoveRange(_currentMoveIndex + 1, _gameHistory.Count - _currentMoveIndex - 1);
        }
        
        _gameHistory.Add(snapshot);
        _liveGameState = snapshot;
        _currentMoveIndex = -1; // Return to live game
    }
    /// <summary>
    /// Navigate to previous move
    /// </summary>
    public BoardStateSnapshot? NavigateBack()
    {
        if (!CanNavigateBack) return null;
        
        if (_currentMoveIndex == -1)
        {
            _currentMoveIndex = _gameHistory.Count - 1;
        }
        else if (_currentMoveIndex > 0)
        {
            _currentMoveIndex--;
        }
        
        return _gameHistory[_currentMoveIndex];
    }
    
    /// <summary>
    /// Navigate to next move
    /// </summary>
    public BoardStateSnapshot? NavigateForward()
    {
        if (!CanNavigateForward) return null;
        
        _currentMoveIndex++;
        
        // If we've reached the end, stay at the last historical move
        // Don't automatically return to live game
        if (_currentMoveIndex >= _gameHistory.Count)
        {
            _currentMoveIndex = _gameHistory.Count - 1;
            return _gameHistory[_currentMoveIndex];
        }
        
        return _gameHistory[_currentMoveIndex];
    }
    
    /// <summary>
    /// Return to current live game state
    /// </summary>
    public BoardStateSnapshot? ReturnToLiveGame()
    {
        _currentMoveIndex = -1;
        return _liveGameState;
    }
    
    /// <summary>
    /// Clear all history
    /// </summary>
    public void ClearHistory()
    {
        _gameHistory.Clear();
        _liveGameState = null;
        _currentMoveIndex = -1;
    }
    
    /// <summary>
    /// Restore board state from snapshot
    /// </summary>
    public void RestoreBoardState(ChessBoardModel board, BoardStateSnapshot snapshot)
    {
        // Clear current board
        foreach (var square in board.Squares)
        {
            square.Piece = null;
        }
        
        // Restore pieces from snapshot
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var pieceSnapshot = snapshot.BoardState[row, col];
                if (pieceSnapshot != null)
                {
                    var square = board.GetSquare(row, col);
                    if (square != null)
                    {
                        square.Piece = CreatePieceFromSnapshot(pieceSnapshot);
                    }
                }
            }
        }
    }
    
    private ChessPiece CreatePieceFromSnapshot(PieceSnapshot snapshot)
    {
        ChessPiece piece = snapshot.Type switch
        {
            PieceType.Pawn => new Pawn(),
            PieceType.Rook => new Rook(),
            PieceType.Knight => new Knight(),
            PieceType.Bishop => new Bishop(),
            PieceType.Queen => new Queen(),
            PieceType.King => new King(),
            _ => throw new ArgumentException($"Unknown piece type: {snapshot.Type}")
        };
        
        piece.Color = snapshot.Color;
        
        // Restore HasMoved state for pieces that track it
        if (piece is King king)
            king.HasMoved = snapshot.HasMoved;
        else if (piece is Rook rook)
            rook.HasMoved = snapshot.HasMoved;
        
        return piece;
    }
    
    private CastlingRightsSnapshot CaptureCastlingRights(CastlingValidator castlingValidator)
    {
        // Will need to expose these properties from CastlingValidator
        // For now, returning default values
        return new CastlingRightsSnapshot();
    }
}
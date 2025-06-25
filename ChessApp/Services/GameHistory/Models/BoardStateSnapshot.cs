using ChessApp.Models.Chess;

namespace ChessApp.Services.GameHistory.Models;


/// <summary>
/// Represents a snapshot of the board state for a specific mvoe
/// </summary>
public class BoardStateSnapshot
{
    public int MoveNumber { get; set; }
    public PieceColor CurrentTurn { get; set; }
    public int HalfMoveCounter { get; set; }

    /// <summary>
    /// Array representing the board state: [row][column] = piece info
    /// </summary>
    public PieceSnapshot?[,] BoardState { get; set; } = new PieceSnapshot?[8, 8];

    /// <summary>
    /// Castling rights snapshot
    /// </summary>
    public CastlingRightsSnapshot CastlingRight { get; set; } = new();
    
    public BoardStateSnapshot(){}

    public BoardStateSnapshot(
        int moveNumber,
        PieceColor currentTurn,
        int halfMoveCounter,
        PieceSnapshot?[,] boardState,
        CastlingRightsSnapshot castlingRight)
    {
        MoveNumber = moveNumber;
        CurrentTurn = currentTurn;
        HalfMoveCounter = halfMoveCounter;
        BoardState = boardState;
        CastlingRight = castlingRight;
    }
}
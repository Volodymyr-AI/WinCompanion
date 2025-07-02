using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Validators.EnPassantValidation;


/// <summary>
/// We check if on current move globally on a board en passant is available
/// </summary>
public class EnPassantValidator
{
    private ChessSquare? _enPassantTargetSquare;
    private int _enPassantMoveNumber = -1;
    private int _currentMoveNumber = 0;
    
    
    /// <summary>
    /// Square where en passant capture can be made (the square behind the pawn that just moved)
    /// </summary>
    public ChessSquare? EnPassantTargetSquare =>
        (_currentMoveNumber == _enPassantMoveNumber + 1) ? _enPassantTargetSquare : null;

    /// <summary>
    /// Updates en passant state after a move
    /// </summary>
    /// <param name="board"></param>
    /// <param name="fromSquare"></param>
    /// <param name="toSquare"></param>
    /// <param name="moveNumber"></param>
    public void UpdateAfterMove(ChessBoardModel board, ChessSquare fromSquare, ChessSquare toSquare, int moveNumber)
    {
        _currentMoveNumber = moveNumber;
        
        // Clear previous en passant opportunity
        _enPassantTargetSquare = null;
        _enPassantMoveNumber = -1;
        
        // Check if this move creates en passant opportunity
        if (fromSquare.Piece is Pawn pawn)
        {
            int rowDifference = Math.Abs(fromSquare.Row - toSquare.Row);
            
            //if pawn moved 2 squares
            if (rowDifference == 2)
            {
                // En passsant targer square is a square the pawn jumped over 
                int targetRow = (fromSquare.Row + toSquare.Row) / 2;
                int targetColumn = fromSquare.Column;
                
                _enPassantTargetSquare = board.GetSquare(targetRow, targetColumn);
                _enPassantMoveNumber = moveNumber;
            }
        }
    }

    /// <summary>
    /// Check if en passant capture is valid
    /// </summary>
    /// <param name="fromSquare"></param>
    /// <param name="toSquare"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    public bool IsEnPassantCapture(ChessSquare fromSquare, ChessSquare toSquare, ChessBoardModel board)
    {
        if(fromSquare.Piece is not Pawn attackingPawn) return false;
        if(EnPassantTargetSquare is null) return false;
        
        //Check if we're capturing to the en passant target square
        if(toSquare.Row != EnPassantTargetSquare.Row || toSquare.Column != EnPassantTargetSquare.Column) return false;
        
        // Check if the attacking pawn is in the correct pos
        int expectedRow = attackingPawn.Color == PieceColor.White ? 3 : 4; // 5th rank for white and 4th for black
        if(fromSquare.Row != expectedRow) return false;
        
        // Check if there's an enemy pawn next to us that just moved 2 squares
        var enemyPawnSquare = board.GetSquare(fromSquare.Row, toSquare.Column);
        if(enemyPawnSquare?.Piece is not Pawn enemyPawn) return false;
        if(enemyPawn.Color == attackingPawn.Color) return false;
        
        return true;
    }

    /// <summary>
    /// Executes en passant capture (removes the captured pawn)
    /// </summary>
    /// <param name="toSquare"></param>
    /// <param name="board"></param>
    public void ExecuteEnPassantCapture(ChessSquare toSquare, ChessBoardModel board)
    {
        if(EnPassantTargetSquare is null) return;
        
        // The captured pawn is not on the target square, but on the same file as target square
        // and same rank as the attacking pawn was
        PieceColor attackingColor = toSquare.Piece?.Color ?? PieceColor.White;
        int capturedPawnRow = attackingColor == PieceColor.White ? toSquare.Row + 1 : toSquare.Row - 1;
        
        var capturedPawnSquare = board.GetSquare(capturedPawnRow, toSquare.Column);
        if (capturedPawnSquare != null)
        {
            capturedPawnSquare.Piece = null;
        }
    }

    /// <summary>
    /// Reset en passant state 
    /// </summary>
    public void Reset()
    {
        _enPassantTargetSquare = null;
        _enPassantMoveNumber = -1;
        _currentMoveNumber = 0;
    }
}
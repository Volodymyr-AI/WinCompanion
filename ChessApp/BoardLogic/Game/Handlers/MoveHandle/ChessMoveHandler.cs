using System.Windows;
using System.Windows.Media;
using ChessApp.BoardLogic.Game.Actions.Highlight;
using ChessApp.BoardLogic.Game.Handlers.SelectHandle;
using ChessApp.BoardLogic.Game.Managers.GameManager;
using ChessApp.BoardLogic.Game.Tracker;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.BoardLogic.Game.Validators.EnPassantValidation;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;
using ChessApp.Services.PieceNotationService.Entity;

namespace ChessApp.BoardLogic.Game.Handlers.MoveHandle;

public class ChessMoveHandler : IChessMoveHandler
{
    private readonly ChessBoardModel _chessBoardModel;
    private readonly CastlingValidator _castlingValidator;
    private readonly EnPassantValidator _enPassantValidator;
    
    private readonly IMoveHighlighter _highlighter;
    private readonly IPieceSelectHandler _pieceSelectHandler;
    private readonly MoveTracker _moveTracker;
    public event Action BoardUpdated;
    public event Action<Move> MoveExecuted;

    public ChessMoveHandler(
        ChessBoardModel boardModel,
        CastlingValidator castlingValidator,
        IMoveHighlighter highlighter,
        IPieceSelectHandler pieceSelectHandler)
    {
        _chessBoardModel = boardModel;
        _castlingValidator = castlingValidator;
        _highlighter = highlighter;
        _pieceSelectHandler = pieceSelectHandler;
        _moveTracker = new MoveTracker(boardModel);
    }
    
    /// <summary>
    /// Handles the movement of a selected chess piece to the specified destination.
    /// 
    /// - Ensures the board is initialized.
    /// - Checks if the move is valid for the selected piece.
    /// - If the King is in check:
    ///   - The King can only move to a safe square.
    ///   - Other pieces can only move if they remove the check.
    /// - If there is no check:
    ///   - Ensures the move does not expose the King to check.
    /// - Handles castling if applicable.
    /// - Executes the move if all conditions are met.
    /// </summary>
    /// <param name="destinationSquare">The target square for the move.</param>
    public void HandlePieceMovement(ChessSquare destinationSquare)
    {
        if (_chessBoardModel?.Squares == null)
        {
            Logging.ShowError("Board is not empty");
            return;
        }
        
        ChessSquare fromSquare = _pieceSelectHandler.SelectedSquare!;
        ChessPiece movingPiece = fromSquare.Piece!;
        PieceColor pieceColor = movingPiece.Color;
        
        bool isPawnPromotion = movingPiece is Pawn && 
                               (destinationSquare.Row == 0 || destinationSquare.Row == 7);
        
        // Record beginning of the move
        _moveTracker.RecordMoveStart(fromSquare);
        
        if (destinationSquare.Piece != null)
        {
            _moveTracker.RecordCapture(destinationSquare.Piece);
        }
        
        bool isKingSideCastle = false;
        bool isQueenSideCastle = false;

        if (IsCastlingMove(_pieceSelectHandler.SelectedSquare!, destinationSquare))
        {
            isKingSideCastle = destinationSquare.Column > _pieceSelectHandler.SelectedSquare!.Column;
            isQueenSideCastle = !isKingSideCastle;
            HandleCastlingMove(_pieceSelectHandler.SelectedSquare!, destinationSquare);
        }
        else
        {
            MovePiece(destinationSquare);
        }
        
        _moveTracker.RecordMoveEnd(destinationSquare, isKingSideCastle, isQueenSideCastle, isPawnPromotion);
        
        // Check if this is checked or mate
        PieceColor opponentColor = pieceColor == PieceColor.White 
            ? PieceColor.Black : PieceColor.White;
        
        bool isCheck = CheckMateValidator.IsKingCheck(_chessBoardModel, opponentColor);
        bool isCheckmate = isCheck && CheckMateValidator.IsCheckmate(_chessBoardModel, opponentColor);
        
        // Create an object of a Move and calling event
        int moveNumber = 1;
        var move = _moveTracker.CreateMove(moveNumber, pieceColor, isCheck, isCheckmate);
        MoveExecuted?.Invoke(move);
    }
    
    /// <summary>
    /// Move the piece selected by the gravel to a new square on the chessboard.
    /// Updates the board state, changes the move, checks the game status (check, mate, stalemate).
    /// </summary>
    /// <param name="destinationSquare">The cell where you want to move the figure.</param>
    private void MovePiece(ChessSquare destinationSquare)
    {
        var movedPiece = _pieceSelectHandler.SelectedSquare!.Piece;
        
        destinationSquare.Piece = movedPiece;
        _pieceSelectHandler.SelectedSquare!.Piece = null;
        // mark King moved
        if (movedPiece is King)
        {
            _castlingValidator.MarkKingMoved(movedPiece.Color);
        }
        // mark Rook moved
        if (movedPiece is Rook)
        {
            _castlingValidator.MarkRookMoved(movedPiece.Color, _pieceSelectHandler.SelectedSquare!.Column);
        }
        
        _pieceSelectHandler.SelectedSquare.IsSelected = false;
        _highlighter.ClearHighlights(_chessBoardModel);

        HandlePawnPromotionMove(destinationSquare);
        _pieceSelectHandler.SetSelectedSquareToNull();
        
        BoardUpdated?.Invoke(); // Inform viewmodel
    }

    /// <summary>
    /// Moves a piece from one square to another without changing the current move.
    /// Used for special moves such as castling.
    /// </summary>
    /// <param name="destinationSquare">The cell where the figure moves.</param>
    /// <param name="source">The cell from which the piece moves.</param>
    private void CastlePieceMove(ChessSquare destinationSquare, ChessSquare sourceSquare)
    {
        destinationSquare.Piece = sourceSquare.Piece;
        sourceSquare.Piece = null;
        sourceSquare.IsSelected = false;

        if (destinationSquare.Piece is Pawn)
        {
            HandlePawnPromotionMove(destinationSquare);
        }
        _highlighter.ClearHighlights(_chessBoardModel);
        BoardUpdated?.Invoke();
    }
    
    /// <summary>
    /// Promote Pawn to Queen if it has reached another edge of a board
    /// </summary>
    private void HandlePawnPromotionMove(ChessSquare selectedSquare)
    {
        if (selectedSquare.Piece is Pawn && (selectedSquare.Row == 0 || selectedSquare.Row == 7))
        {
            selectedSquare.Piece = new Queen {Color = selectedSquare.Piece.Color};
        }
    }
    
    /// <summary>
    /// Checks if the move player wants to make is castling
    /// </summary>
    /// <returns></returns>
    private bool IsCastlingMove(ChessSquare selectedSquare, ChessSquare destinationSquare)
    {
        return selectedSquare?.Piece is King 
               && Math.Abs(selectedSquare.Column - destinationSquare.Column) == 2;
    }

    /// <summary>
    /// Provides a castling logic 
    /// </summary>
    /// <param name="kingSquare">Square where King stands ( for success must be on a start pos )</param>
    /// <param name="destination">Square where King should stand for castling</param>
    private void HandleCastlingMove(ChessSquare kingSquare, ChessSquare destination)
    {
        int step = destination.Column > kingSquare.Column ? 1 : -1; // Check in which direction we should move to make castling
        int rookColumn = (step == 1) ? 7 : 0; // Check column of Rook
        ChessSquare rookSquare = _chessBoardModel.Squares.FirstOrDefault(sq => sq.Row == kingSquare.Row && sq.Column == rookColumn); // Square of rook must be same to king's row and be on a start column to castle successfully
        if (rookSquare == null) 
        {
            Logging.ShowError("No rook found for castling.");
            return;
        }
        if (!_castlingValidator.CanCastle(kingSquare, rookSquare, _chessBoardModel))
        {
            Logging.ShowError("Invalid castling");
            return;
        }
        CastlePieceMove(destination, kingSquare); // moving King piece to a new place deleting it from the old square

        int rookNewColumn = destination.Column - step; // Count new column for Rook
        ChessSquare rookNewSquare = _chessBoardModel.Squares.First(sq => sq.Row == rookSquare.Row && sq.Column == rookNewColumn); // Searching for the square where Rook should step
        CastlePieceMove(rookNewSquare, rookSquare); // Moving Rook to a rookNewSquare
        
        // Check new position of a King
        if (destination.Piece == null || !(destination.Piece is King))
        {
            Logging.ShowError("King is missing after castling.");
            return;
        }
        
        _pieceSelectHandler.SetSelectedSquareToNull(); // unselect king square
    }
}
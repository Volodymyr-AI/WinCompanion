using System.Windows;
using System.Windows.Media;
using ChessApp.BoardLogic.Game.Actions.Highlight;
using ChessApp.BoardLogic.Game.Managers.GameManager;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Game.Handlers.MoveHandle;

public class ChessMoveHandler : IChessMoveHandler
{
    private ChessSquare _selectedSquare;
    private GameStatusManager _gameStatusManager;
    
    private readonly ChessBoardModel _chessBoardModel;
    private readonly CastlingValidator _castlingValidator;
    
    private readonly IMoveHighlighter _highlighter;
    private readonly IMoveValidator _moveValidator;
    public event Action BoardUpdated;

    public ChessMoveHandler(
        ChessBoardModel boardModel,
        CastlingValidator castlingValidator,
        GameStatusManager gameStatusManager,
        IMoveHighlighter highlighter,
        IMoveValidator moveValidator)
    {
        _chessBoardModel = boardModel;
        _castlingValidator = castlingValidator;
        _gameStatusManager = gameStatusManager;
        _highlighter = highlighter;
        _moveValidator = moveValidator;
    }
    
    
    /// <summary>
    /// Entry point of a class handling different piece movements
    /// </summary>
    /// <param name="clickedSquare"></param>
    public void OnSquareClicked(ChessSquare clickedSquare)
    {
        if (clickedSquare == null)
            return;

        // Check game status before doing anything
        if (_gameStatusManager.CheckGameStatus())
            return;
        if (_selectedSquare == null)
            SelectPiece(clickedSquare);
        else if (_selectedSquare == clickedSquare)
            UnselectPiece(clickedSquare);
        else
            HandlePieceMovement(clickedSquare);
    }
    
    private void SelectPiece(ChessSquare clickedSquare)
    {
        if (clickedSquare.Piece != null && clickedSquare.Piece.Color == _gameStatusManager.CurrentTurn)
        {
            clickedSquare.IsSelected = true;
            clickedSquare.Background = Brushes.LightGreen;
            _highlighter.HighlightMoves(clickedSquare, _chessBoardModel, _castlingValidator);
            _selectedSquare = clickedSquare;
        }
    }
    private void UnselectPiece(ChessSquare clickedSquare)
    {
        _selectedSquare.IsSelected = false;
        _selectedSquare.Background = _selectedSquare.BaseBackground;
        _highlighter.ClearHighlights(_chessBoardModel);
        _selectedSquare = null;
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
    private void HandlePieceMovement(ChessSquare destinationSquare)
    {
        if (_chessBoardModel?.Squares == null)
        {
            Logging.ShowError("Board is not empty");
            return;
        }

        if (!_moveValidator.IsMoveValid(
                _chessBoardModel,
                _selectedSquare,
                destinationSquare,
                _gameStatusManager.CurrentTurn,
                out var errorMessage))
        {
            Logging.ShowError(errorMessage);
            UnselectPiece(_selectedSquare);
            return;
        }
        
        if(IsCastlingMove(_selectedSquare, destinationSquare))
            HandleCastling(_selectedSquare, destinationSquare);
        else
            MovePiece(destinationSquare);
    }
    
    /// <summary>
    /// Move the piece selected by the gravel to a new square on the chessboard.
    /// Updates the board state, changes the move, checks the game status (check, mate, stalemate).
    /// </summary>
    /// <param name="destinationSquare">The cell where you want to move the figure.</param>
    private void MovePiece(ChessSquare destinationSquare)
    {
        var movedPiece = _selectedSquare.Piece;
        
        destinationSquare.Piece = movedPiece;
        _selectedSquare.Piece = null;
        // mark King moved
        if (movedPiece is King)
        {
            _castlingValidator.MarkKingMoved(movedPiece.Color);
        }
        // mark Rook moved
        if (movedPiece is Rook)
        {
            _castlingValidator.MarkRookMoved(movedPiece.Color, _selectedSquare.Column);
        }
        
        _selectedSquare.IsSelected = false;
        _highlighter.ClearHighlights(_chessBoardModel);

        HandlePawnPromotion(destinationSquare);
        _selectedSquare = null;
        
        BoardUpdated?.Invoke(); // Inform viewmodel
        _gameStatusManager.CheckGameStatus();
    }

    /// <summary>
    /// Moves a piece from one square to another without changing the current move.
    /// Used for special moves such as castling.
    /// </summary>
    /// <param name="destinationSquare">The cell where the figure moves.</param>
    /// <param name="source">The cell from which the piece moves.</param>
    private void CastlePiece(ChessSquare destinationSquare, ChessSquare sourceSquare)
    {
        destinationSquare.Piece = sourceSquare.Piece;
        sourceSquare.Piece = null;
        sourceSquare.IsSelected = false;

        if (destinationSquare.Piece is Pawn)
        {
            HandlePawnPromotion(destinationSquare);
        }
        _highlighter.ClearHighlights(_chessBoardModel);
        BoardUpdated?.Invoke();
    }
    
    /// <summary>
    /// Promote Pawn to Queen if it has reached another edge of a board
    /// </summary>
    private void HandlePawnPromotion(ChessSquare selectedSquare)
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
        return _selectedSquare.Piece is King 
               && Math.Abs(_selectedSquare.Column - destinationSquare.Column) == 2;
    }

    /// <summary>
    /// Provides a castling logic 
    /// </summary>
    /// <param name="kingSquare">Square where King stands ( for success must be on a start pos )</param>
    /// <param name="destination">Square where King should stand for castling</param>
    private void HandleCastling(ChessSquare kingSquare, ChessSquare destination)
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
        CastlePiece(destination, kingSquare); // moving King piece to a new place deleting it from the old square

        int rookNewColumn = destination.Column - step; // Count new column for Rook
        ChessSquare rookNewSquare = _chessBoardModel.Squares.First(sq => sq.Row == rookSquare.Row && sq.Column == rookNewColumn); // Searching for the square where Rook should step
        CastlePiece(rookNewSquare, rookSquare); // Moving Rook to a rookNewSquare
        
        // Check new position of a King
        if (destination.Piece == null || !(destination.Piece is King))
        {
            Logging.ShowError("King is missing after castling.");
            return;
        }
        
        _selectedSquare = null; // unselect king square
        _gameStatusManager.CurrentTurn = _gameStatusManager.Opponent(_gameStatusManager.CurrentTurn); // giving turn to opponent
    }
    
    public void SetGameHandler(GameStatusManager gameStatusManager)
    {
        _gameStatusManager = gameStatusManager;
    }
}
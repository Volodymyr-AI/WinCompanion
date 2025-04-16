using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ChessApp.BoardLogic.Validators;
using ChessApp.Models.Board;
using ChessApp.Models.Chess.Pieces;

namespace ChessApp.BoardLogic.Handlers;

public class ChessMoveHandler : IChessMoveHandler
{
    private ChessSquare selectedSquare;
    
    private readonly ChessBoardModel _chessBoardModel;
    private readonly CastlingValidator _castlingValidator;
    private GameHandler _gameHandler;
    
    public event Action BoardUpdated;

    public ChessMoveHandler(ChessBoardModel boardModel, CastlingValidator castlingValidator, GameHandler gameHandler)
    {
        _chessBoardModel = boardModel;
        _castlingValidator = castlingValidator;
        _gameHandler = gameHandler;
    }
    
    public void OnSquareClicked(ChessSquare clickedSquare)
    {
        if (clickedSquare == null)
            return;

        // Check game status before doing anything
        if (_gameHandler.CheckGameStatus())
            return;
        if (selectedSquare == null)
            SelectPiece(clickedSquare);
        else if (selectedSquare == clickedSquare)
            UnselectPiece(clickedSquare);
        else
            HandlePieceMovement(clickedSquare);
    }
    
    private void SelectPiece(ChessSquare clickedSquare)
    {
        if (clickedSquare.Piece != null && clickedSquare.Piece.Color == _gameHandler.CurrentTurn)
        {
            clickedSquare.IsSelected = true;
            clickedSquare.Background = Brushes.LightGreen;
            HighlightPossibleMoves(clickedSquare);
            selectedSquare = clickedSquare;
        }
    }
    private void UnselectPiece(ChessSquare clickedSquare)
    {
        selectedSquare.IsSelected = false;
        selectedSquare.Background = selectedSquare.BaseBackground;
        ClearHighlightedMoves();
        selectedSquare = null;
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
        if (_chessBoardModel == null || _chessBoardModel.Squares == null)
        {
            MessageBox.Show("Board is not initialized!");
            return;
        }

        if (!selectedSquare.Piece.IsValidMove(selectedSquare, destinationSquare, _chessBoardModel.Squares))
        {
            MessageBox.Show("Invalid move");
            UnselectPiece(selectedSquare);
            return;
        }

        if (CheckMateValidator.IsKingCheck(_chessBoardModel, _gameHandler.CurrentTurn))
        {
            if (selectedSquare.Piece is King)
            {
                if (!CheckMateValidator.IsSafeForKingToMove(_chessBoardModel, selectedSquare, destinationSquare))
                {
                    MessageBox.Show("Invalid move, King is still under check!");
                    UnselectPiece(selectedSquare);
                    return;
                }
            }
            else
            {
                if (!CheckMateValidator.DoesMoveDefendKing(_chessBoardModel, selectedSquare, destinationSquare))
                {
                    MessageBox.Show("Invalid move, this move doesn't remove the check!");
                    UnselectPiece(selectedSquare);
                    return;
                }
            }
        }
        else
        {
            if (CheckMateValidator.DoesMoveExposeKingToCheck(_chessBoardModel, selectedSquare, destinationSquare))
            {
                MessageBox.Show("Invalid move, this move exposes the King to check!");
                UnselectPiece(selectedSquare);
                return;
            }
        }

        if (selectedSquare.Piece is King && Math.Abs(selectedSquare.Column - destinationSquare.Column) == 2)
        {
            HandleCastling(selectedSquare, destinationSquare);
        }
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
        var movedPiece = selectedSquare.Piece;
        
        destinationSquare.Piece = movedPiece;
        selectedSquare.Piece = null;
        // mark King moved
        if (movedPiece is King)
        {
            _castlingValidator.MarkKingMoved(movedPiece.Color);
        }
        // mark Rook moved
        if (movedPiece is Rook)
        {
            _castlingValidator.MarkRookMoved(movedPiece.Color, selectedSquare.Column);
        }
        
        selectedSquare.IsSelected = false;
        ClearHighlightedMoves();

        HandlePawnPromotion(destinationSquare);
        selectedSquare = null;
        
        BoardUpdated?.Invoke(); // Inform viewmodel
        _gameHandler.CheckGameStatus();
    }

    /// <summary>
    /// Moves a piece from one square to another without changing the current move.
    /// Used for special moves such as castling.
    /// </summary>
    /// <param name="destinationSquare">The cell where the figure moves.</param>
    /// <param name="source">The cell from which the piece moves.</param>
    private void MovePiece(ChessSquare destinationSquare, ChessSquare sourceSquare)
    {
        destinationSquare.Piece = sourceSquare.Piece;
        sourceSquare.Piece = null;
        sourceSquare.IsSelected = false;

        if (destinationSquare.Piece is Pawn)
        {
            HandlePawnPromotion(destinationSquare);
        }
        
        BoardUpdated?.Invoke();
    }
    
    private void HandlePawnPromotion(ChessSquare clickedSquare)
    {
        if (clickedSquare.Piece is Pawn && (clickedSquare.Row == 0 || clickedSquare.Row == 7))
        {
            clickedSquare.Piece = new Queen {Color = clickedSquare.Piece.Color};
        }
    }

    private void HandleCastling(ChessSquare kingSquare, ChessSquare destination)
    {
        int step = destination.Column > kingSquare.Column ? 1 : -1; // Check in which direction we should move to make castling
        int rookColumn = (step == 1) ? 7 : 0; // Check column of Rook
        ChessSquare rookSquare = _chessBoardModel.Squares.FirstOrDefault(sq => sq.Row == kingSquare.Row && sq.Column == rookColumn); // Square of rook must be same to king's row and be on a start column to castle successfully
        if (rookSquare == null) 
        {
            MessageBox.Show("No rook found for castling.");
            return;
        }
        if (!_castlingValidator.CanCastle(kingSquare, rookSquare, _chessBoardModel))
        {
            MessageBox.Show("Invalid castling");
            return;
        }

        King king = (King)kingSquare.Piece; // Save the King before moving
        MovePiece(destination, kingSquare); // moving King piece to a new place deleting it from the old square

        int rookNewColumn = destination.Column - step; // Count new column for Rook
        ChessSquare rookNewSquare = _chessBoardModel.Squares.First(sq => sq.Row == rookSquare.Row && sq.Column == rookNewColumn); // Searching for the square where Rook should step
        Rook rook = rookSquare.Piece as Rook;
        MovePiece(rookNewSquare, rookSquare); // Moving Rook to a rookNewSquare
        
        // Check new position of a King
        if (destination.Piece == null || !(destination.Piece is King))
        {
            MessageBox.Show("King is missing after castling.");
            return;
        }
        
        
        
        selectedSquare = null; // unselect king square
        _gameHandler.CurrentTurn = _gameHandler.Opponent(_gameHandler.CurrentTurn); // giving turn to opponent
    }
    
    public void SetGameHandler(GameHandler gameHandler)
    {
        _gameHandler = gameHandler;
    }

    /// <summary>
    /// Highlight selected pieces possible moves
    /// </summary>
    private void HighlightPossibleMoves(ChessSquare selectedSquare)
    {
        List<ChessSquare> possibleMoves = MoveGenerator.GetPossibleMoves(selectedSquare, _chessBoardModel);
        
        Debug.WriteLine("Possible moves count: " + possibleMoves.Count);
        foreach (var square in possibleMoves)
        {
            if (selectedSquare.Piece is King &&
                CheckMateValidator.IsKingCheckAfterMove(_chessBoardModel, selectedSquare, square))
            {
                square.Background = square.BaseBackground;
            }
            else if (square.Piece != null)
            {
                square.Background = Brushes.LightCoral;
            }
            else
            {
                square.Background = Brushes.LightBlue;
            }
        }
    }
    
    // <summary>
    /// Clears the highlighting of all squares by resetting 
    /// their Background property to the BaseBackground.
    /// </summary>
    private void ClearHighlightedMoves()
    {
        foreach (var square in _chessBoardModel.Squares)
        {
            square.Background = square.BaseBackground;
        }
    }
}
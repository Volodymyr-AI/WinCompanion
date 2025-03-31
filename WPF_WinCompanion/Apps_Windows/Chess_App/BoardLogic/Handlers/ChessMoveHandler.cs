using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class ChessMoveHandler : IChessMoveHandler
{
    private ChessSquare selectedSquare;
    
    private PieceColor _currentTurn;
    private readonly ChessBoardModel _chessBoardModel;
    private readonly CastlingValidator _castlingValidator;
    private GameHandler _gameHandler;
    
    public event Action BoardUpdated;

    public ChessMoveHandler(ChessBoardModel boardModel, CastlingValidator castlingValidator, GameHandler gameHandler)
    {
        _chessBoardModel = boardModel;
        _castlingValidator = castlingValidator;
        _gameHandler = gameHandler;
        _currentTurn = PieceColor.White;
    }
    
    public void OnSquareClicked(ChessSquare clickedSquare)
    {
        if (clickedSquare == null)
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
        if (clickedSquare.Piece != null && clickedSquare.Piece.Color == _currentTurn)
        {
            clickedSquare.IsSelected = true;
            selectedSquare = clickedSquare;
            Console.WriteLine($"{clickedSquare.Piece.Type} selected: ({clickedSquare.Row}, {clickedSquare.Column})");
        }
    }
    private void UnselectPiece(ChessSquare clickedSquare)
    {
        selectedSquare.IsSelected = false;
        selectedSquare = null;
        Console.WriteLine($"{clickedSquare.Piece.Type} unselected");
    }
    
    private void HandlePieceMovement(ChessSquare destinationSquare)
    {
        if (_chessBoardModel == null || _chessBoardModel.Squares == null)
        {
            MessageBox.Show("Board is not initialized!");
            return;
        }
        
        if (selectedSquare.Piece.IsValidMove(selectedSquare, destinationSquare, _chessBoardModel.Squares))
        {
            if (selectedSquare.Piece is King && Math.Abs(selectedSquare.Column - destinationSquare.Column) == 2)
                HandleCastling(selectedSquare, destinationSquare);
            else
                MovePiece(destinationSquare);
        }
        else
        {
            MessageBox.Show("Invalid move");
            selectedSquare.IsSelected = false;
            selectedSquare = null;
        }
    }
    
    /// <summary>
    /// Move the piece selected by the gravel to a new square on the chessboard.
    /// Updates the board state, changes the move, checks the game status (check, mate, stalemate).
    /// </summary>
    /// <param name="destinationSquare">The cell where you want to move the figure.</param>
    private void MovePiece(ChessSquare destinationSquare)
    {
        destinationSquare.Piece = selectedSquare.Piece;
        selectedSquare.Piece = null;
        selectedSquare.IsSelected = false;

        HandlePawnPromotion(destinationSquare);
    
        selectedSquare = null;
        _currentTurn = _gameHandler.Opponent(_currentTurn);

        BoardUpdated?.Invoke(); // Inform viewmodel
        Console.WriteLine($"{destinationSquare.Piece.Type} moved");

        //GameHandler.CheckGameStatus(_chessBoardModel, _currentTurn);
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
        Console.WriteLine($"{destinationSquare.Piece.Type} moved to ({destinationSquare.Row}, {destinationSquare.Column})");
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
        
        _castlingValidator.MarkKingMoved(king.Color);
        _castlingValidator.MarkRookMoved(rook.Color, rookSquare.Column);
        
        selectedSquare = null; // unselect king square
        _currentTurn = _currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White; // giving turn to opponent
    }
    
    public void SetGameHandler(GameHandler gameHandler)
    {
        _gameHandler = gameHandler;
    }
}
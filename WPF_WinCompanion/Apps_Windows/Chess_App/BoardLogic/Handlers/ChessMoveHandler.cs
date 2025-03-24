using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class ChessMoveHandler
{
    private ChessSquare _selectedSquare;
    private PieceColor _currentTurn;
    private readonly ChessBoardModel _chessBoardModel;
    private readonly CastlingValidator _castlingValidator;
    
    public event Action BoardUpdated;

    public ChessMoveHandler(ChessBoardModel boardModel, PieceColor startingTurn, CastlingValidator castlingValidator)
    {
        _chessBoardModel = boardModel;
        _currentTurn = startingTurn;
        _castlingValidator = castlingValidator;
    }
    
    public void OnSquareClicked(ChessSquare clickedSquare)
    {
        if (clickedSquare == null)
        {
            return;
        }

        if (_selectedSquare == null)
        {
            HandlePieceSelection(clickedSquare);
        }
        else if (_selectedSquare == clickedSquare)
        {
            HandleUnselect(clickedSquare);
        }
        else
        {
            HandlePieceMovement(clickedSquare);
        }
    }
    
    private PieceColor OpponentColor(PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    
    private void HandlePieceSelection(ChessSquare clickedSquare)
    {
        if (clickedSquare.Piece != null && clickedSquare.Piece.Color == _currentTurn)
        {
            clickedSquare.IsSelected = true;
            _selectedSquare = clickedSquare;
            Console.WriteLine($"{clickedSquare.Piece.Type} selected: ({clickedSquare.Row}, {clickedSquare.Column})");
        }
    }
    
    private void HandleUnselect(ChessSquare clickedSquare)
    {
        _selectedSquare.IsSelected = false;
        _selectedSquare = null;
        Console.WriteLine($"{clickedSquare.Piece.Type} unselected");
    }
    
    private void HandlePieceMovement(ChessSquare clickedSquare)
    {
        if (_chessBoardModel == null || _chessBoardModel.Squares == null)
        {
            MessageBox.Show("Board is not initialized!");
            return;
        }
        
        if (_selectedSquare.Piece.IsValidMove(_selectedSquare, clickedSquare, _chessBoardModel.Squares))
        {
            if (_selectedSquare.Piece is King && Math.Abs(_selectedSquare.Column - clickedSquare.Column) == 2)
            {
                HandleCastling(_selectedSquare, clickedSquare);
            }
            else
            {
                MovePiece(clickedSquare);
            }
        }
        else
        {
            MessageBox.Show("Invalid move");
            _selectedSquare.IsSelected = false;
            _selectedSquare = null;
        }
    }
    
    private void MovePiece(ChessSquare clickedSquare)
    {
        clickedSquare.Piece = _selectedSquare.Piece;
        _selectedSquare.Piece = null;
        _selectedSquare.IsSelected = false;

        HandlePawnPromotion(clickedSquare);
    
        _selectedSquare = null;
        _currentTurn = OpponentColor(_currentTurn);

        BoardUpdated?.Invoke(); // Inform viewmodel
        Console.WriteLine($"{clickedSquare.Piece.Type} moved");

        CheckForKingStatus();
    }

    private void MovePiece(ChessSquare destination, ChessSquare source)
    {
        destination.Piece = source.Piece;
        source.Piece = null;
        source.IsSelected = false;

        if (destination.Piece is Pawn)
        {
            HandlePawnPromotion(destination);
        }
        
        BoardUpdated?.Invoke();
        Console.WriteLine($"{destination.Piece.Type} moved to ({destination.Row}, {destination.Column})");
    }
    
    private void HandlePawnPromotion(ChessSquare clickedSquare)
    {
        if (clickedSquare.Piece is Pawn && (clickedSquare.Row == 0 || clickedSquare.Row == 7))
        {
            clickedSquare.Piece = new Queen {Color = clickedSquare.Piece.Color};
        }
    }
    
    private void CheckForKingStatus()
    {
        if (CheckMateValidator.IsKingCheck(_chessBoardModel, _currentTurn))
        {
            if (CheckMateValidator.IsCheckmate(_chessBoardModel, _currentTurn))
            {
                MessageBox.Show($"{_currentTurn} - Checkmate");
                Debug.WriteLine($"{_currentTurn} is checkmated!");
            }
            else
            {
                MessageBox.Show($"{_currentTurn} - King Check");
                Debug.WriteLine($"{_currentTurn} is in check!");
            }
        }
    }

    private void HandleCastling(ChessSquare kingSquare, ChessSquare destination)
    {
        int step = destination.Column > kingSquare.Column ? 1 : -1;
        int rookColumn = (step == 1) ? 7 : 0; // Check where Rook is
        ChessSquare rookSquare = _chessBoardModel.Squares.FirstOrDefault(sq => sq.Row == kingSquare.Row && sq.Column == rookColumn);
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
        
        MovePiece(destination, kingSquare);

        int rookNewColumn = destination.Column - step;
        ChessSquare rookNewSquare = _chessBoardModel.Squares.First(sq => sq.Row == rookSquare.Row && sq.Column == rookNewColumn);
        MovePiece(rookNewSquare, rookSquare);
        
        // Проверка на наличие фигуры на клетке короля
        if (kingSquare.Piece == null)
        {
            //MessageBox.Show("No piece on the king's square.");
            return;
        }

        // Проверка на ладью перед перемещением
        if (rookSquare.Piece == null || !(rookSquare.Piece is Rook))
        {
            MessageBox.Show("Invalid rook on the square.");
            return;
        }
        

        _castlingValidator.MarkKingMoved(kingSquare.Piece.Color);
        _castlingValidator.MarkRookMoved(rookSquare.Piece.Color, rookSquare.Column);
    }
}
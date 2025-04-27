using System.Windows.Media;
using ChessApp.BoardLogic.Game.Actions.Highlight;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Handlers.SelectHandle;

public class PieceSelectHandler : IPieceSelectHandler
{
    private readonly IMoveHighlighter _highlighter;
    private readonly ChessBoardModel _chessBoardModel;
    private readonly CastlingValidator _castlingValidator;
    
    private ChessSquare _selectedSquare;

    public PieceSelectHandler(
        IMoveHighlighter highlighter,
        ChessBoardModel chessBoardModel,
        CastlingValidator castlingValidator)
    {
        _highlighter = highlighter;
        _chessBoardModel = chessBoardModel;
        _castlingValidator = castlingValidator;
    }
    
    public void SelectPiece(ChessSquare clickedSquare)
    {
        clickedSquare.IsSelected = true;
        clickedSquare.Background = Brushes.LightGreen;
        _highlighter.HighlightMoves(clickedSquare, _chessBoardModel, _castlingValidator);
        _selectedSquare = clickedSquare;
        
    }
    public void UnselectPiece(ChessSquare clickedSquare)
    {
        _selectedSquare.IsSelected = false;
        _selectedSquare.Background = _selectedSquare.BaseBackground;
        _highlighter.ClearHighlights(_chessBoardModel);
        _selectedSquare = null;
    }
    
    public ChessSquare? SelectedSquare => _selectedSquare;
    public bool HasSelectedPiece => _selectedSquare != null;
    public void SetSelectedSquareToNull() => _selectedSquare = null;
}
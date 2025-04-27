using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Handlers.SelectHandle;
using ChessApp.BoardLogic.Game.Managers.GameManager;
using ChessApp.BoardLogic.Game.Validators.CheckmateValidation;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.BoardLogic.Game.Validators.StalemateValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Coordinators.Game;

public class GameCoordinator : IGameCoordinator
{
    private readonly IChessMoveHandler _moveHandler;
    private readonly IGameStatusManager _gameStatusManager;
    private readonly IMoveValidator _moveValidator;
    private readonly IPieceSelectHandler _pieceSelectHandler;

    private readonly ChessBoardModel _board;
    
    public GameCoordinator(
        IChessMoveHandler moveHandler,
        IGameStatusManager gameStatusManager,
        IMoveValidator moveValidator,
        IPieceSelectHandler pieceSelectHandler,
        ChessBoardModel board)
    {
        _moveHandler = moveHandler;
        _gameStatusManager = gameStatusManager;
        _moveValidator = moveValidator;
        _pieceSelectHandler = pieceSelectHandler;
        _board = board;
    }
    
    public void OnSquareClicked(ChessSquare clickedSquare)
    {
        if (clickedSquare == null)
            return;

        if (_gameStatusManager.CheckGameStatus())
        {
            _gameStatusManager.TrySetGameOver();
            return;
        }
        
        if (_gameStatusManager.IsGameOver)
            return;

        if (!_pieceSelectHandler.HasSelectedPiece)
        {
            TrySelectPiece(clickedSquare);
        }
        else if (_pieceSelectHandler.SelectedSquare == clickedSquare)
        {
            _pieceSelectHandler.UnselectPiece(clickedSquare);
        }
        else
        {
            CoordinateMove(clickedSquare);
            ShowGameResult();
        }
    }
    
    private void TrySelectPiece(ChessSquare clickedSquare)
    {
        if (clickedSquare.Piece != null && clickedSquare.Piece.Color == _gameStatusManager.CurrentTurn)
        {
            _pieceSelectHandler.SelectPiece(clickedSquare);
        }
        else
        {
            Logging.ShowError("Invalid selection: it's not your piece.");
        }
    }

    private void CoordinateMove(ChessSquare destination)
    {
        var from = _pieceSelectHandler.SelectedSquare!;
        
        if (!_moveValidator.IsMoveValid(
                board: _board,
                from: from,
                to: destination,
                turn: _gameStatusManager.CurrentTurn,
                out string errorMessage))
        {
            Logging.ShowError(errorMessage);
            _pieceSelectHandler.UnselectPiece(from);
            return;
        }

        _moveHandler.HandlePieceMovement(destination);

        if (_gameStatusManager.CheckGameStatus())
        {
            bool gameOver = _gameStatusManager.TrySetGameOver();
            if (gameOver)
            {
                Logging.ShowInfo("Game was successfully ended.");
            }
            else
            {
                Logging.ShowWarning("Game is already over, no action taken");
            }
        }
        else
        {
            _gameStatusManager.SwitchTurn();
        }
    }
    
    private void ShowGameResult()
    {
        if (CheckMateValidator.IsCheckmate(_gameStatusManager.BoardModel, _gameStatusManager.CurrentTurn))
        {
            Logging.ShowInfo($"Checkmate! {_gameStatusManager.CurrentTurn} lost.");
        }
        
        if (StalemateValidator.IsStalemate(_gameStatusManager.BoardModel, _gameStatusManager.CurrentTurn))
        {
            Logging.ShowInfo("Stalemate!");
        }
    }
}
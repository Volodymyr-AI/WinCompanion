using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Manager.GameManager;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.Infrastructure.Log;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Game.Coordinator;

public class ChessTurnCoordinator
{
    private readonly IChessMoveHandler _moveHandler;
    private readonly IGameSessionManager _gameManager;

    public ChessTurnCoordinator(IChessMoveHandler moveHandler, IGameSessionManager gameManager)
    {
        _moveHandler = moveHandler;
        _gameManager = gameManager;

        _moveHandler.BoardUpdated += OnMoveHandled;
    }

    private void OnMoveHandled()
    {
        _gameManager.OnMoveMade();
    }
}
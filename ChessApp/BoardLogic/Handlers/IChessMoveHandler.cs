using System;
using ChessApp.Models.Board;

namespace ChessApp.BoardLogic.Handlers;

public interface IChessMoveHandler
{
    event Action BoardUpdated;
    void OnSquareClicked(ChessSquare clickedSquare);
}
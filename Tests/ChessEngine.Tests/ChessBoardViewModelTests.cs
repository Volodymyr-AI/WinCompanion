using ChessApp.Models.Chess;
using ChessApp.Models.Chess.Pieces;
using ChessApp.ViewModels;

namespace ChessEngine.Tests;

public class ChessBoardViewModelTests
{
    [Fact]
    public void RestartCommand_Should_ResetBoard_And_SetTurnToWhite()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // We simulate a change of move (for example, we put the black king)
        viewModel.BoardModel.Squares.First(s => s.Row == 7 && s.Column == 4).Piece = new King { Color = PieceColor.Black };

        // Act
        viewModel.RestartCommand.Execute(null);

        // Assert
        Assert.Equal(PieceColor.White, viewModel.CurrentTurn);
        Assert.IsType<King>(viewModel.BoardModel.GetSquare(7, 4)!.Piece);
        Assert.Equal(PieceColor.White, viewModel.BoardModel.GetSquare(7, 4)!.Piece!.Color);
    }
}
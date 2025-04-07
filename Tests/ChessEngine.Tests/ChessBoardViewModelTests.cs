using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;
using WPF_WinCompanion.Apps_Windows.Chess_App.ViewModels;

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
        Assert.Equal(PieceColor.White, viewModel.CurrentTurn); // білий повинен ходити після рестарту
        Assert.IsType<King>(viewModel.BoardModel.GetSquare(7, 4)!.Piece);
        Assert.Equal(PieceColor.White, viewModel.BoardModel.GetSquare(7, 4)!.Piece!.Color);
    }
}
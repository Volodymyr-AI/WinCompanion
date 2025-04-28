using AppViewModels.Chess;
using ChessApp.Models.Chess;
using FluentAssertions;

namespace ViewModelTests.Chess;

public class ChessViewModelTests
{
    [Fact]
    public void ChessBoardViewModel_Starts_With_White_Turn()
    {
        //Arrange
        var viewModel = new ChessBoardViewModel();
        
        //Act
        var currentTurn = viewModel.CurrentTurn;
        
        //Assert
        currentTurn.Should().Be(PieceColor.White);
    }
}
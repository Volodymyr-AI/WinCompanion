using ChessApp.BoardLogic;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace ChessEngine.Tests;

public class KingCheckTests
{
    [Fact]
    public void IsKingCheck_ReturnsTrue_WhenEnemyPieceAttacksKing()
    {
        // Arrange
        var board = ChessBoardInitializer.InitializeEmptyBoard();

        var king = new King();
        king.Color = PieceColor.White;
        var queen = new Queen();
        queen.Color = PieceColor.Black;

        board.GetSquare(7, 4)!.Piece = king;   // E1
        board.GetSquare(0, 4)!.Piece = queen;  // E8

        // Act
        var result = CheckMateValidator.IsKingCheck(board, PieceColor.White);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsKingCheck_ReturnsFalse_WhenNoThreat()
    {
        // Arrange
        var board = ChessBoardInitializer.InitializeEmptyBoard();

        var king = new King();
        king.Color = PieceColor.White;
        var pawn = new Pawn();
        pawn.Color = PieceColor.Black;

        board.GetSquare(7, 4)!.Piece = king;
        board.GetSquare(0, 3)!.Piece = pawn;

        // Act
        var result = CheckMateValidator.IsKingCheck(board, PieceColor.White);

        // Assert
        Assert.False(result);
    }
}
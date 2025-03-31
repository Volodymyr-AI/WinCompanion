using System.Diagnostics;
using System.Windows;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class GameHandler
{
    public static void CheckGameStatus(ChessBoardModel board, PieceColor currentTurn)
    {
        if (CheckMateValidator.IsKingCheck(board, currentTurn))
        {
            if (CheckMateValidator.IsCheckmate(board, currentTurn))
            {
                MessageBox.Show($"Checkmate");
                Debug.WriteLine($"{currentTurn} is checkmated!");
            }
            else
            {
                MessageBox.Show($"King Check");
                Debug.WriteLine($"{currentTurn} is in check!");
            }
        }
        else if(StalemateValidator.IsStalemate(board, currentTurn))
        {
            MessageBox.Show($"Game finished with Stalemate");
            Debug.WriteLine($"Game is stalemated!");
        }
    }
    
    public PieceColor Opponent( PieceColor color)
        => color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}
using System.Windows;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;

public class GameHandler
{
    public ChessBoardModel ChessBoardModel { get; set; } = new();
    public void StartNewGame()
    {
        ChessBoardInitializer.InitializeBoard(ChessBoardModel);
    }
}
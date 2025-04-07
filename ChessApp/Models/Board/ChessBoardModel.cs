using System.Collections.ObjectModel;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;

public class ChessBoardModel
{
    public ObservableCollection<ChessSquare> Squares { get; } = new();
    public List<string> Letters { get; } 
        = ["A", "B", "C", "D", "E", "F", "G", "H"];
    public List<string> Numbers { get; } 
        = ["8", "7", "6", "5", "4", "3", "2", "1"];

    public ChessSquare? GetSquare(int squareRow, int squareColumn)
    {
        return Squares.FirstOrDefault(s => s.Row == squareRow && s.Column == squareColumn);
    }

    public bool IsInsideBoard(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }
}
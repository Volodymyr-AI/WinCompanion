using System.Windows;
using WPF_WinCompanion.Apps_Windows.Chess_App.ViewModels;

namespace WPF_WinCompanion.Apps_Views.Chess_App.Views;

public partial class ChessWindow : Window
{
    public ChessWindow()
    {
        InitializeComponent();
        DataContext = new ChessBoardViewModel();
    }
}
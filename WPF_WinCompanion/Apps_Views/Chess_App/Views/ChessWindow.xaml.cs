using System.Windows;
using ChessApp.ViewModels;

namespace WPF_WinCompanion.Apps_Views.Chess_App.Views;

public partial class ChessWindow : Window
{
    public ChessWindow()
    {
        InitializeComponent();
        DataContext = new ChessBoardViewModel();
    }
}
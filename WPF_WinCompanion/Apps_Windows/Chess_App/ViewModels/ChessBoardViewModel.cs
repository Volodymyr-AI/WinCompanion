using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Handlers;
using WPF_WinCompanion.Apps_Windows.Chess_App.BoardLogic.Validators;
using WPF_WinCompanion.Apps_Windows.Chess_App.Commands;
using WPF_WinCompanion.Apps_Windows.Chess_App.Helpers;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Board;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess;
using WPF_WinCompanion.Apps_Windows.Chess_App.Models.Chess.Pieces;

namespace WPF_WinCompanion.Apps_Windows.Chess_App.ViewModels;

public class ChessBoardViewModel : INotifyPropertyChanged
{
    public ChessBoardModel BoardModel { get; init; } = new();
    public ICommand SquareClickCommand { get; }
    
    private GameHandler _gameHandler;
    private readonly ChessMoveHandler _chessMoveHandler;
    public ChessBoardViewModel()
    {
        ChessBoardInitializer.InitializeBoard(BoardModel);
        _gameHandler = new GameHandler();
        _chessMoveHandler = new ChessMoveHandler(BoardModel,  new CastlingValidator(),  _gameHandler);
        _chessMoveHandler.BoardUpdated += () => OnPropertyChanged(nameof(BoardModel));

        SquareClickCommand = new RelayCommand(par =>
        {
            if (par is ChessSquare square)
            {
                _chessMoveHandler.OnSquareClicked(square);
            }
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
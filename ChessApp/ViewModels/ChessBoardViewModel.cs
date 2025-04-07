using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ChessApp.BoardLogic;
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
    public ICommand RestartCommand { get; }
    
    public PieceColor CurrentTurn => _gameHandler.CurrentTurn;
    
    private readonly IChessMoveHandler _moveHandler;
    private readonly GameHandler _gameHandler;

    public ChessBoardViewModel()
    {
        ChessBoardInitializer.InitializeBoard(BoardModel);

        _moveHandler = new ChessMoveHandler(BoardModel, new CastlingValidator(), null);
        _gameHandler = new GameHandler(BoardModel, _moveHandler);
        _gameHandler.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(GameHandler.CurrentTurn))
            {
                Debug.WriteLine($"🔄 UI notified about CurrentTurn change: {_gameHandler.CurrentTurn}");
                OnPropertyChanged(nameof(CurrentTurn));
            }
        };
        
        ((ChessMoveHandler)_moveHandler).SetGameHandler(_gameHandler);

        _moveHandler.BoardUpdated += () => OnPropertyChanged(nameof(BoardModel));

        SquareClickCommand = new RelayCommand(par =>
        {
            if (par is ChessSquare square)
            {
                _moveHandler.OnSquareClicked(square);
            }
        });

        RestartCommand = new RelayCommand(_ => _gameHandler.RestartGame());
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
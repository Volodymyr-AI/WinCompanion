using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using ChessApp.BoardLogic;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game;
using ChessApp.BoardLogic.Game.Validators;
using ChessApp.BoardLogic.Handlers;
using ChessApp.BoardLogic.Interfaces;
using ChessApp.Commands;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.ViewModels;

public class ChessBoardViewModel : INotifyPropertyChanged
{
    public ChessBoardModel BoardModel { get; init; } = new();
    public ICommand SquareClickCommand { get; }
    public ICommand RestartCommand { get; }
    
    public PieceColor CurrentTurn => _gameHandler.CurrentTurn;
    
    private readonly IChessMoveHandler _moveHandler;
    private readonly IMoveHighlighter _highlighter;
    private readonly GameHandler _gameHandler;

    public ChessBoardViewModel()
    {
        ChessBoardInitializer.InitializeBoard(BoardModel);

        var castlingValidator = new CastlingValidator();
        _highlighter = new MoveHighlighter();
        _moveHandler = new ChessMoveHandler(BoardModel, castlingValidator, null, _highlighter);
        _gameHandler = new GameHandler(BoardModel, _moveHandler,castlingValidator);
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using ChessApp.BoardLogic;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game;
using ChessApp.BoardLogic.Game.Actions;
using ChessApp.BoardLogic.Game.Actions.Highlight;
using ChessApp.BoardLogic.Game.Handlers;
using ChessApp.BoardLogic.Game.Handlers.GameHandle;
using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Validators;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.Infrastructure.Commands;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;

namespace ChessApp.ViewModels;

/// <summary>
/// View‑model that hosts the board state, reacts on square clicks
/// and delegates logic to GameHandler / ChessMoveHandler.
/// </summary>
public class ChessBoardViewModel : INotifyPropertyChanged
{
    #region public‑API -------------------------------------------------------

    /// <summary> Model that collects all 64 squares and chess pieces </summary>
    public ChessBoardModel BoardModel { get; } = new();

    /// <summary> Current turn color. Indicates whose turn right now </summary>
    public PieceColor CurrentTurn => _gameHandler.CurrentTurn;

    /// <summary> Board square click </summary>
    public ICommand SquareClickCommand { get; }

    /// <summary> Start a new game command </summary>
    public ICommand RestartCommand  { get; }

    #endregion

    #region ctor -------------------------------------------------------------

    public ChessBoardViewModel()
    {
        /* 1. -------------  low‑level helpers --------------------------------*/
        ChessBoardInitializer.InitializeBoard(BoardModel);

        var castling      = new CastlingValidator();
        var moveValidator = new MoveValidator();
        var highlighter   = new MoveHighlight();

        /* 2. -------------  domain services ----------------------------------*/
        // Move‑handler still does not know about GameHandler → transmit null
        _moveHandler = new ChessMoveHandler(
            BoardModel,
            castling,
            gameHandler : null,
            highlighter,
            moveValidator);

        _gameHandler = new GameHandler(
            BoardModel,
            _moveHandler,
            castling);

        // «circular‑ctor»
        ((ChessMoveHandler)_moveHandler).SetGameHandler(_gameHandler);

        /* 3. -------------  data‑binding callbacks ---------------------------*/
        _gameHandler.PropertyChanged += OnGameHandlerPropertyChanged;
        _moveHandler.BoardUpdated    += () => OnPropertyChanged(nameof(BoardModel));

        /* 4. -------------  UI commands --------------------------------------*/
        SquareClickCommand = new RelayCommand(obj =>
        {
            if (obj is ChessSquare square)
                _moveHandler.OnSquareClicked(square);
        });

        RestartCommand = new RelayCommand(_ => _gameHandler.RestartGame());
    }

    #endregion

    #region private‑helpers --------------------------------------------------

    private readonly IChessMoveHandler _moveHandler;
    private readonly GameHandler       _gameHandler;

    private void OnGameHandlerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameHandler.CurrentTurn))
            OnPropertyChanged(nameof(CurrentTurn));
    }

    #endregion

    #region INotifyPropertyChanged ------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ChessApp.BoardLogic.Board;
using ChessApp.BoardLogic.Game.Actions.Highlight;
using ChessApp.BoardLogic.Game.Coordinators.Game;
using ChessApp.BoardLogic.Game.Handlers.MoveHandle;
using ChessApp.BoardLogic.Game.Handlers.SelectHandle;
using ChessApp.BoardLogic.Game.Managers.GameManager;
using ChessApp.BoardLogic.Game.Validators.CastlingValidation;
using ChessApp.BoardLogic.Game.Validators.MoveValidation;
using ChessApp.Infrastructure.Commands;
using ChessApp.Models.Board;
using ChessApp.Models.Chess;
using ChessApp.Services.GameHistory.Models;
using ChessApp.Services.PieceNotationService.Entity;
using ChessApp.Services.PieceNotationService.Notation;
using ChessApp.Services.PieceNotationService.Notation.Types;

namespace AppViewModels.Chess;

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
    public PieceColor CurrentTurn => _gameStatusManager.CurrentTurn;

    /// <summary> Board square click </summary>
    public ICommand SquareClickCommand { get; }

    /// <summary> Start a new game command </summary>
    public ICommand RestartCommand  { get; }
    
    /// <summary> History of game moves</summary>
    public ObservableCollection<MoveHistoryItem> MoveHistory { get; } = new();

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
        _pieceSelectHandler = new PieceSelectHandler(
            highlighter,
            BoardModel,
            castling);
        
        _moveHandler = new ChessMoveHandler(
            BoardModel,
            castling,
            highlighter,
            _pieceSelectHandler);

        _gameStatusManager = new GameStatusManager(
            BoardModel,
            castling);

        _gameCoordinator = new GameCoordinator(
            _moveHandler,
            _gameStatusManager,
            moveValidator,
            _pieceSelectHandler,
            BoardModel);
        
        // Initialize notation formatter
        var disambiguationService = new DisambiguationService(BoardModel);
        _moveFormatter = new MoveNotationFormatter(BoardModel);

        /* 3. -------------  data‑binding callbacks ---------------------------*/
        _gameStatusManager.PropertyChanged += OnGameStatusManagerPropertyChanged;
        _moveHandler.BoardUpdated    += () => OnPropertyChanged(nameof(BoardModel));
        _moveHandler.MoveExecuted += OnMoveExecuted;

        /* 4. -------------  UI commands --------------------------------------*/
        SquareClickCommand = new RelayCommand(obj =>
        {
            if (obj is ChessSquare square)
                _gameCoordinator.OnSquareClicked(square);
        });

        RestartCommand = new RelayCommand(_ =>
        {
            _gameStatusManager.RestartGame();
            MoveHistory.Clear();
            _moveCount = 1;
            _lastMoveColor = PieceColor.Black;
            CommandManager.InvalidateRequerySuggested();
        });
    }

    #endregion

    #region private‑helpers --------------------------------------------------

    private readonly IPieceSelectHandler     _pieceSelectHandler;
    private readonly IChessMoveHandler       _moveHandler;
    private readonly IGameStatusManager      _gameStatusManager;
    private readonly IGameCoordinator        _gameCoordinator;
    private readonly IMoveNotationFormatter  _moveFormatter;

    private int _moveCount = 1;
    private PieceColor _lastMoveColor = PieceColor.Black;

    private void OnGameStatusManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameStatusManager.CurrentTurn))
            OnPropertyChanged(nameof(CurrentTurn));
    }

    private void OnMoveExecuted(Move move)
    {
        if (move.Color == "White")
        {
            MoveHistory.Add(new MoveHistoryItem(_moveCount, _moveFormatter.Format(move), null));
        }
        else
        {
            if (MoveHistory.Count > 0)
            {
                var lastItem = MoveHistory.Last();
                lastItem.BlackMove = _moveFormatter.Format(move);
                
                _moveCount++;
            }
            else
            {
                MoveHistory.Add(new MoveHistoryItem(_moveCount, "", _moveFormatter.Format(move)));
                _moveCount++;
            }
        }
    }

    #endregion

    #region INotifyPropertyChanged ------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion
}
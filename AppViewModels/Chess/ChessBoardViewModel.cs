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
using ChessApp.Models.Game.Enums;
using ChessApp.Services.GameHistory;
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
    
    /// <summary> Current halfmove counter for 50-move rule </summary>
    public int HalfMoveCounter => _gameStatusManager.HalfMoveCounter;

    /// <summary> Board square click </summary>
    public ICommand SquareClickCommand { get; }

    /// <summary> Start a new game command </summary>
    public ICommand RestartCommand  { get; }
    /// <summary> Claim draw by 50-move rule </summary>
    public ICommand ClaimFiftyMoveDrawCommand { get; }
    
    /// <summary> History of game moves</summary>
    public ObservableCollection<MoveHistoryItem> MoveHistory { get; } = new();
    
    /// <summary> Indicates if fifty-move draw can be claimed </summary>
    public bool CanClaimFiftyMoveDraw => _gameStatusManager.CanClaimFiftyMoveDraw();
    
    /// <summary> Command to show game menu </summary>
    public ICommand ShowMenuCommand { get; }
    
    /// <summary> Command to navigate to previous move </summary>
    public ICommand NavigateBackCommand { get; }

    /// <summary> Command to navigate to next move </summary>
    public ICommand NavigateForwardCommand { get; }
    
    /// <summary> Command to return to live game </summary>
    public ICommand ReturnToLiveGameCommand { get; }
    
    /// <summary> Indicates if currently viewing history </summary>
    public bool IsViewingHistory => _historyManager.IsViewingHistory;

    /// <summary> Can navigate back in history </summary>
    public bool CanNavigateBack => _historyManager.CanNavigateBack;

    /// <summary> Can navigate forward in history </summary>
    public bool CanNavigateForward => _historyManager.CanNavigateForward;

    #endregion
    
    #region Menu Properties

    /// <summary> Current menu state </summary>
    private MenuState _currentMenuState = MenuState.MainMenu;
    public MenuState CurrentMenuState
    {
        get => _currentMenuState;
        set
        {
            if (_currentMenuState != value)
            {
                _currentMenuState = value;
                OnPropertyChanged(nameof(CurrentMenuState));
                OnPropertyChanged(nameof(ShowGameMenu));
                OnPropertyChanged(nameof(ShowMainMenu));
                OnPropertyChanged(nameof(ShowSoloSettings));
                OnPropertyChanged(nameof(ShowAISettings));
                OnPropertyChanged(nameof(ShowOnlineSettings));
            }
        }
    }

    /// <summary> Show any menu overlay </summary>
    public bool ShowGameMenu => CurrentMenuState != MenuState.Hidden;

    /// <summary> Show main menu </summary>
    public bool ShowMainMenu => CurrentMenuState == MenuState.MainMenu;

    /// <summary> Show solo game settings </summary>
    public bool ShowSoloSettings => CurrentMenuState == MenuState.SoloGameSettings;

    /// <summary> Show AI game settings </summary>
    public bool ShowAISettings => CurrentMenuState == MenuState.AIGameSettings;

    /// <summary> Show online game settings </summary>
    public bool ShowOnlineSettings => CurrentMenuState == MenuState.OnlineGameSettings;

    #endregion
    
    #region Menu Commands

    /// <summary> Command to show solo game settings </summary>
    public ICommand ShowSoloSettingsCommand { get; }

    /// <summary> Command to show AI game settings </summary>
    public ICommand ShowAISettingsCommand { get; }

    /// <summary> Command to show online game settings </summary>
    public ICommand ShowOnlineSettingsCommand { get; }

    /// <summary> Command to go back to main menu </summary>
    public ICommand BackToMainMenuCommand { get; }

    /// <summary> Command to confirm solo game with settings </summary>
    public ICommand ConfirmSoloGameCommand { get; }

    /// <summary> Command to confirm AI game with settings </summary>
    public ICommand ConfirmAIGameCommand { get; }

    /// <summary> Command to connect online game </summary>
    public ICommand ConnectOnlineGameCommand { get; }

    #endregion

    #region ctor -------------------------------------------------------------

    public ChessBoardViewModel()
    {
        /* 1. -------------  low‑level helpers --------------------------------*/
        ChessBoardInitializer.InitializeBoard(BoardModel);

        _castlingValidator = new CastlingValidator();
        var moveValidator = new MoveValidator();
        var highlighter   = new MoveHighlight();

        /* 2. -------------  domain services ----------------------------------*/
        _pieceSelectHandler = new PieceSelectHandler(
            highlighter,
            BoardModel,
            _castlingValidator);
        
        _moveHandler = new ChessMoveHandler(
            BoardModel,
            _castlingValidator,
            highlighter,
            _pieceSelectHandler);

        _gameStatusManager = new GameStatusManager(
            BoardModel,
            _castlingValidator);

        _gameCoordinator = new GameCoordinator(
            _moveHandler,
            _gameStatusManager,
            moveValidator,
            _pieceSelectHandler,
            BoardModel);
        
        // Initialize notation formatter
        var disambiguationService = new DisambiguationService(BoardModel);
        _moveFormatter = new MoveNotationFormatter(BoardModel);
        _historyManager = new GameHistoryManager();

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
            _historyManager.ClearHistory();
            _moveCount = 1;
            _lastMoveColor = PieceColor.Black;
            OnPropertyChanged(nameof(HalfMoveCounter));
            OnPropertyChanged(nameof(CanClaimFiftyMoveDraw));
            OnPropertyChanged(nameof(CanNavigateBack));
            OnPropertyChanged(nameof(CanNavigateForward));
            OnPropertyChanged(nameof(IsViewingHistory));
            CommandManager.InvalidateRequerySuggested();
        });
        
        ClaimFiftyMoveDrawCommand = new RelayCommand(_ =>
        {
            if (_gameStatusManager.CanClaimFiftyMoveDraw())
            {
                _gameStatusManager.TrySetGameOver();
            }
        }, _ => _gameStatusManager.CanClaimFiftyMoveDraw());
        
        // Game menu commands
        ShowSoloSettingsCommand = new RelayCommand(_ =>
        {
            CurrentMenuState = MenuState.SoloGameSettings;
        });

        ShowAISettingsCommand = new RelayCommand(_ =>
        {
            CurrentMenuState = MenuState.AIGameSettings;
        });

        ShowOnlineSettingsCommand = new RelayCommand(_ =>
        {
            CurrentMenuState = MenuState.OnlineGameSettings;
        });

        BackToMainMenuCommand = new RelayCommand(_ =>
        {
            CurrentMenuState = MenuState.MainMenu;
        });

        ConfirmSoloGameCommand = new RelayCommand(_ =>
        {
            StartGame(GameMode.Solo);
            CurrentMenuState = MenuState.Hidden;
        });

        ConfirmAIGameCommand = new RelayCommand(_ =>
        {
            StartGame(GameMode.AI);
            CurrentMenuState = MenuState.Hidden;
        });

        ConnectOnlineGameCommand = new RelayCommand(_ =>
        {
            // TODO: Implement online connection logic
            StartGame(GameMode.Online);
            CurrentMenuState = MenuState.Hidden;
        });

        ShowMenuCommand = new RelayCommand(_ =>
        {
            CurrentMenuState = MenuState.MainMenu;
        });
        
        NavigateBackCommand = new RelayCommand(_ =>
        {
            NavigateToHistoryMove(-1);
        }, _ => CanNavigateBack);

        NavigateForwardCommand = new RelayCommand(_ =>
        {
            NavigateToHistoryMove(1);
        }, _ => CanNavigateForward);
        
        ReturnToLiveGameCommand = new RelayCommand(_ =>
        {
            ReturnToCurrentGame();
        }, _ => IsViewingHistory);

        // Show menu at startup
        CurrentMenuState = MenuState.MainMenu;
    }

    #endregion

    #region private‑helpers --------------------------------------------------

    private readonly IPieceSelectHandler     _pieceSelectHandler;
    private readonly IChessMoveHandler       _moveHandler;
    private readonly IGameStatusManager      _gameStatusManager;
    private readonly IGameCoordinator        _gameCoordinator;
    private readonly IMoveNotationFormatter  _moveFormatter;
    private readonly GameHistoryManager      _historyManager;
    private readonly CastlingValidator       _castlingValidator;

    private int _moveCount = 1;
    private PieceColor _lastMoveColor = PieceColor.Black;
    private bool _showGameMenu;
    private GameMode _currentGameMode = GameMode.Solo;

    private void OnGameStatusManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameStatusManager.CurrentTurn))
            OnPropertyChanged(nameof(CurrentTurn));
    }

    private void OnMoveExecuted(Move move)
    {
        _gameStatusManager.UpdateFiftyMoveRule(move);
        
        // Capture board state for history
        _historyManager.CaptureCurrentState(
            BoardModel, 
            _gameStatusManager.CurrentTurn, 
            _gameStatusManager.HalfMoveCounter,
            _castlingValidator,
            _moveCount);
        
        OnPropertyChanged(nameof(HalfMoveCounter));
        OnPropertyChanged(nameof(CanClaimFiftyMoveDraw));
        OnPropertyChanged(nameof(CanNavigateBack));
        OnPropertyChanged(nameof(CanNavigateForward));
        OnPropertyChanged(nameof(IsViewingHistory));
        
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
    
    private void StartGame(GameMode gameMode)
    {
        _currentGameMode = gameMode;
        CurrentMenuState = MenuState.Hidden;
        
        // Reset game state
        _gameStatusManager.RestartGame();
        MoveHistory.Clear();
        _historyManager.ClearHistory();
        _moveCount = 1;
        _lastMoveColor = PieceColor.Black;
        OnPropertyChanged(nameof(HalfMoveCounter));
        OnPropertyChanged(nameof(CanClaimFiftyMoveDraw));
        OnPropertyChanged(nameof(CanNavigateBack));
        OnPropertyChanged(nameof(CanNavigateForward));
        OnPropertyChanged(nameof(IsViewingHistory));
        CommandManager.InvalidateRequerySuggested();

        // TODO: Initialize specific game mode logic here
        switch (gameMode)
        {
            case GameMode.Solo:
                // Solo game logic - already implemented
                break;
            case GameMode.AI:
                // TODO: Initialize AI opponent
                break;
            case GameMode.Online:
                // TODO: Initialize online game connection
                break;
        }
    }
    
    private void NavigateToHistoryMove(int direction)
    {
        BoardStateSnapshot? snapshot = direction < 0 
            ? _historyManager.NavigateBack() 
            : _historyManager.NavigateForward();
        
        if (snapshot != null)
        {
            _historyManager.RestoreBoardState(BoardModel, snapshot);
            OnPropertyChanged(nameof(BoardModel));
        }
        
        
        OnPropertyChanged(nameof(CanNavigateBack));
        OnPropertyChanged(nameof(CanNavigateForward));
        OnPropertyChanged(nameof(IsViewingHistory));
        CommandManager.InvalidateRequerySuggested();
    }
    
    private void ReturnToCurrentGame()
    {
        var liveState = _historyManager.ReturnToLiveGame();

        if (liveState != null)
        {
            _historyManager.RestoreBoardState(BoardModel, liveState);
        }
        
        // Restore current live game state
        //TODO:Will need to implement a way to store and restore the current live state
        // For now => reinitialize from the current game manager state
        OnPropertyChanged(nameof(BoardModel));
        OnPropertyChanged(nameof(CanNavigateBack));
        OnPropertyChanged(nameof(CanNavigateForward));
        OnPropertyChanged(nameof(IsViewingHistory));
        CommandManager.InvalidateRequerySuggested();
    }
    
    #endregion

    #region INotifyPropertyChanged ------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion
}
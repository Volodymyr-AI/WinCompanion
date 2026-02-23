# WinCompanion

> A modular WPF desktop companion application featuring multiple mini-apps — Chess, Weather, Finance Tracker, and Notes — all accessible from a unified, elegant launcher.

---

## Table of Contents

- [Overview](#overview)
- [Solution Architecture](#solution-architecture)
- [Projects](#projects)
- [Chess Application](#chess-application)
  - [Features](#features)
  - [Architecture](#architecture)
  - [Game Engine](#game-engine)
  - [Move Validation Pipeline](#move-validation-pipeline)
  - [Castling](#castling)
  - [History Navigation](#history-navigation)
  - [Move Notation (SAN)](#move-notation-san)
  - [50-Move Rule](#50-move-rule)
  - [Game Menu System](#game-menu-system)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Testing](#testing)

---

## Overview

**WinCompanion** is a Windows desktop application built with WPF (.NET 9) that serves as a personal companion suite. The main window acts as a home screen launcher with app icons, a live clock, and a real-time currency ticker (USD/EUR/GBP via NBU API). From there, users can open individual sub-applications in separate windows.

The flagship sub-application is a fully-featured **Chess game** with a custom engine, move history, algebraic notation, and game mode selection.

---

## Solution Architecture

```
WinCompanion.sln
│
├── WPF_WinCompanion          # Main launcher UI (WPF)
│
├── Apps/
│   └── ChessApp              # Domain logic, models, validators, services
│
├── ViewModels/
│   └── AppViewModels         # MVVM ViewModels (platform-agnostic)
│
└── Tests/
    ├── ChessEngine.Tests     # Unit tests for chess engine (xUnit)
    └── ViewModelTests        # Unit tests for ViewModels (xUnit + FluentAssertions)
```

The architecture follows a clean **MVVM** pattern with a strict separation between:

- **Domain layer** (`ChessApp`) — models, validators, move generators, game logic
- **ViewModel layer** (`AppViewModels`) — data-binding logic, commands, observable state
- **View layer** (`WPF_WinCompanion`) — XAML UI, no business logic

---

## Projects

| Project | Type | Description |
|---|---|---|
| `WPF_WinCompanion` | WPF App | Main launcher window, all UI views |
| `ChessApp` | Class Library | Chess engine, models, validators, services |
| `AppViewModels` | Class Library | MVVM ViewModels for all sub-apps |
| `ChessEngine.Tests` | xUnit Test | Engine-level tests (check, checkmate) |
| `ViewModelTests` | xUnit Test | ViewModel-level tests |

---

## Chess Application

The Chess application is a complete two-player chess implementation running entirely in-process, with no external engine dependencies. It supports the full FIDE rule set for standard chess.

### Features

| Feature | Status |
|---|---|
| Full piece movement rules | ✅ |
| Check detection | ✅ |
| Checkmate detection | ✅ |
| Stalemate detection | ✅ |
| Castling (kingside & queenside) | ✅ |
| Pawn promotion (auto-queen) | ✅ |
| 50-move rule draw claim | ✅ |
| Standard Algebraic Notation (SAN) | ✅ |
| Move history display | ✅ |
| Board history navigation (back/forward) | ✅ |
| Move highlighting (legal moves shown) | ✅ |
| Game mode selection menu | ✅ |
| AI Opponent | 🚧 In Progress |
| Online Multiplayer | 🚧 In Progress |

---

### Architecture

The chess sub-app is composed of several coordinated layers:

```
ChessBoardViewModel
        │
        ▼
  GameCoordinator          ← orchestrates click → select → move flow
   ├── PieceSelectHandler  ← manages piece selection & highlight
   ├── ChessMoveHandler    ← executes moves, fires events
   └── GameStatusManager   ← turn management, game-over detection
        │
        ▼
  Validators & Generators
   ├── MoveValidator       ← legal move gate before execution
   ├── MoveGenerator       ← generates pseudo-legal moves per piece type
   ├── CheckMateValidator  ← check, checkmate, stalemate detection
   ├── CastlingValidator   ← castling rights & legality
   └── FiftyMoveRuleValidator
```

#### Key Classes

| Class | Responsibility |
|---|---|
| `ChessBoardViewModel` | Central ViewModel; owns all commands and observable properties |
| `GameCoordinator` | Routes square-click events to select or move logic |
| `ChessMoveHandler` | Physically executes moves on the board model; emits `MoveExecuted` event |
| `PieceSelectHandler` | Tracks the currently selected square and delegates highlighting |
| `GameStatusManager` | Tracks current turn, game-over state, and 50-move counter |
| `MoveValidator` | Validates a move request before execution |
| `MoveGenerator` | Computes all pseudo-legal moves for any piece type |
| `CheckMateValidator` | Detects check, checkmate, and whether a move exposes the king |
| `GameHistoryManager` | Captures and restores board state snapshots for history navigation |
| `MoveNotationFormatter` | Formats a `Move` object into Standard Algebraic Notation |

---

### Game Engine

#### Board Representation

The board is an `ObservableCollection<ChessSquare>` of 64 squares stored in row-major order (row 0 = rank 8, row 7 = rank 1). Each `ChessSquare` holds:

- `Row`, `Column` — zero-based coordinates
- `Piece` — nullable `ChessPiece` (notifies UI on change via `INotifyPropertyChanged`)
- `Background`, `BaseBackground` — brushes for selection / highlight rendering

#### Piece Hierarchy

```
ChessPiece (abstract)
├── Pawn
├── Knight
├── Bishop
├── Rook
├── Queen
└── King
```

Each piece implements `IsValidMove(from, to, board)` for its own movement rules, independently of game state (no check awareness at this level).

#### Move Flow

```
User clicks square
        │
        ▼
GameCoordinator.OnSquareClicked(square)
        │
  [No piece selected?] → TrySelectPiece → PieceSelectHandler.SelectPiece
        │
  [Piece selected]     → MoveValidator.IsMoveValid
                                │
                         [Valid] → ChessMoveHandler.HandlePieceMovement
                                │
                         MoveExecuted event → ViewModel records history & notation
                                │
                         GameStatusManager.CheckGameStatus → SwitchTurn / GameOver
```

---

### Move Validation Pipeline

`MoveValidator.IsMoveValid` enforces three layers of checks:

1. **Piece ownership** — the moving piece must belong to the current player.
2. **Piece movement rules** — `piece.IsValidMove(from, to, board)` must return `true`.
3. **King safety** — evaluated differently based on current game state:
   - If the king is currently **in check**: the move must remove the check (`DoesMoveDefendKing` or `IsSafeForKingToMove`).
   - If the king is **not in check**: the move must not expose the king (`DoesMoveExposeKingToCheck`).

All check simulations work by temporarily mutating the board, running `IsKingCheck`, then reverting — no board copies are created.

#### Checkmate Detection

`CheckMateValidator.IsCheckmate` determines checkmate in three steps:

1. Confirm the king is currently in check.
2. Check whether the king has any safe escape squares.
3. If the king cannot escape, check whether any allied piece can **capture the attacker** or **block the attack ray** (`CanDefendKing`).

If double-check is detected (two attackers simultaneously), only a king move can resolve it — blocking is not considered.

---

### Castling

Castling is handled by `CastlingValidator` and tracked per-color and per-rook-column.

**Conditions checked before allowing castling:**

- Neither the king nor the target rook has previously moved (`_kingMoved`, `_rookMoved` dictionaries).
- All squares between the king and rook are empty.
- The king is not currently in check.
- The king does not pass through or land on a square that is under attack.

When castling is executed in `ChessMoveHandler`, two separate piece moves are performed:
- The king moves two squares toward the rook.
- The rook jumps to the square the king passed over.

Both `MarkKingMoved` and `MarkRookMoved` are called after castling so future castling with the same pieces is disallowed.

---

### History Navigation

`GameHistoryManager` maintains a snapshot list of board states:

- After each move, `CaptureCurrentState` serialises the board into a `BoardStateSnapshot` (an `[8,8]` array of `PieceSnapshot` structs containing piece type, color, and `HasMoved` flag).
- Navigation commands (`NavigateBackCommand` / `NavigateForwardCommand`) call `NavigateBack()` / `NavigateForward()` and restore the appropriate snapshot via `RestoreBoardState`.
- A "Live" button is always available while viewing history, which calls `ReturnToLiveGame()` and restores the last captured live state.

> ⚠️ While viewing history, board interaction and game-state-modifying commands (Restart, Menu) are disabled via WPF bindings on `IsViewingHistory`.

---

### Move Notation (SAN)

Move notation is handled by a composable formatter pipeline in `MoveNotationFormatter`.

The formatter iterates through a list of `INotationPartProvider` implementations:

| Provider | Handles |
|---|---|
| `CastleNotationProvider` | Castling → `O-O` / `O-O-O` |
| `PawnNotationProvider` | Pawn moves → `e4`, `exd5`, `e8=Q` |
| `PieceNotationProvider` | All other pieces → `Nf3`, `Rxd8`, `Bxe5` |

A shared `SuffixProvider` appends `+` for check and `#` for checkmate.

**Disambiguation** is handled by `DisambiguationService`: when two pieces of the same type can move to the same square, the file letter (column) is appended, or the rank (row) if files are identical, or both if necessary (e.g., `Raa1`).

---

### 50-Move Rule

`FiftyMoveRuleValidator` tracks a half-move counter that:

- **Resets to 0** on any pawn move or capture.
- **Increments by 1** on all other moves.
- Triggers a draw condition when the counter reaches **100 half-moves** (50 full moves).

The `CanClaimFiftyMoveDraw` button appears in the UI when the threshold is met and the game is not already over, letting the active player formally claim the draw.

---

### Game Menu System

The menu system is driven by a `MenuState` enum and a `CurrentMenuState` property on the ViewModel. Visibility of overlay panels is controlled purely via WPF `DataTrigger` bindings — no code-behind.

```
MenuState
├── Hidden             ← Board is fully interactive
├── MainMenu           ← Choose game mode
├── SoloGameSettings   ← Time control, hints
├── AIGameSettings     ← Difficulty, color, time
└── OnlineGameSettings ← Server, player name, room ID
```

While any menu is visible, the board and side panel are blurred (`BlurEffect`) and disabled via data triggers, ensuring clean UX separation.

---

## Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | WPF (.NET 9, `net9.0-windows7.0`) |
| Language | C# 13 |
| UI Theme | Material Design In XAML (`MaterialDesignThemes 5.1.0`) |
| Data Binding | MVVM via `INotifyPropertyChanged` + `RelayCommand` |
| JSON | `Newtonsoft.Json 13.0.3` |
| Testing | xUnit 2.9, FluentAssertions 8.2, Moq 4.20 |
| Build | MSBuild / .NET SDK |

---

## Project Structure

```
📁 WinCompanion/
├── 📁 WPF_WinCompanion/          # Main WPF application
│   ├── MainWindow.xaml           # Launcher home screen
│   ├── 📁 Apps_Views/
│   │   ├── 📁 Chess_App/
│   │   │   └── Views/
│   │   │       ├── ChessWindow.xaml
│   │   │       └── Additional/
│   │   │           ├── GameMenuOverlay.xaml
│   │   │           └── GameTypes/
│   │   │               ├── SoloGameSettings.xaml
│   │   │               ├── AIGameSettings.xaml
│   │   │               └── OnlineGameSettings.xaml
│   │   ├── 📁 Weather_App/
│   │   ├── 📁 FinanceTracker_App/
│   │   └── 📁 Notes_App/
│   ├── 📁 Controls/              # Shared controls (MarqueeControl)
│   └── 📁 ViewModel/             # MainViewModel (clock, currency)
│
├── 📁 ChessApp/                  # Chess domain library
│   ├── 📁 BoardLogic/
│   │   ├── Board/                # Initializer, generator
│   │   └── Game/
│   │       ├── Coordinators/
│   │       ├── Generators/       # MoveGenerator
│   │       ├── Handlers/         # MoveHandle, SelectHandle
│   │       ├── Managers/         # GameStatusManager
│   │       ├── Tracker/          # MoveTracker
│   │       └── Validators/       # Check, Castling, Stalemate, FiftyMove, Move
│   ├── 📁 Models/
│   │   ├── Board/                # ChessBoardModel, ChessSquare
│   │   ├── Chess/                # ChessPiece, PieceColor, PieceType, Pieces/
│   │   └── Game/Enums/           # GameMode, MenuState
│   ├── 📁 Services/
│   │   ├── GameHistory/          # Snapshots, history manager
│   │   └── PieceNotationService/ # SAN formatter, providers
│   └── 📁 Infrastructure/
│       ├── Commands/             # RelayCommand
│       ├── Helpers/              # Value converters
│       └── Log/                  # MessageBox logger
│
├── 📁 AppViewModels/
│   └── Chess/ChessBoardViewModel.cs
│
└── 📁 Tests/
    ├── ChessEngine.Tests/        # KingCheckTests, etc.
    └── ViewModelTests/           # ChessViewModelTests, etc.
```

---

## Testing

Tests are split into two projects:

**`ChessEngine.Tests`** — pure engine tests with no UI dependency:
- `KingCheckTests` — verifies that `IsKingCheck` correctly detects attack and non-attack scenarios.

**`ViewModelTests`** — ViewModel integration tests using `FluentAssertions` and `Moq`:
- `ChessViewModelTests` — verifies initial state (white starts first, etc.).

Run all tests:

```bash
dotnet test
```

---

<p align="center">
  Built with ♟️ and ☕ using WPF + .NET 9
</p>

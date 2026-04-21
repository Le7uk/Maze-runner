# 🧩 Maze Runner

A top-down maze game built with **.NET 10 / C# / WinForms** as a university laboratory project.

## 🎮 How to Play

Navigate your character (blue circle) from the **Start (S)** cell to the **Exit (★)** cell.

| Key | Action |
|-----|--------|
| `W` / `↑` | Move Up |
| `S` / `↓` | Move Down |
| `A` / `←` | Move Left |
| `D` / `→` | Move Right |

## ⭐ Star Rating

Each level awards 1–3 stars based on completion time:

| Stars | Condition |
|-------|-----------|
| ★★★ | Completed under the fast threshold |
| ★★   | Completed under the slow threshold |
| ★     | Completed at any time |

Star ratings are session-only — they reset when you close the game.

## 🗺️ Levels

| Level | Grid Size | Description |
|-------|-----------|-------------|
| 1 | 11 × 11 | Beginner |
| 2 | 15 × 15 | Easy |
| 3 | 19 × 19 | Medium |
| 4 | 23 × 23 | Hard |
| 5 | 27 × 27 | Very Hard |
| 6 | 31 × 31 | Expert |

Mazes are randomly generated each time using the **Recursive Backtracker** (DFS) algorithm — no two runs are the same!

## 🏗️ Architecture

```
MazeRunner.sln
├── MazeRunner.Core/          # Business logic (no UI dependencies)
│   ├── Models/               # Cell, Player, Level
│   ├── Services/             # MazeGenerator, GameEngine, LevelConfig
│   ├── Session/              # SessionData (in-memory star ratings)
│   └── Interfaces/           # IGameEngine, Direction
│
├── MazeRunner.App/           # WinForms UI
│   ├── Forms/                # MainMenuForm, LevelSelectForm, GameForm, ResultForm
│   └── Rendering/            # SpriteRenderer (all graphics drawn via GDI+)
│
└── MazeRunner.Tests/         # xUnit unit tests (56 tests)
    ├── MazeGeneratorTests.cs
    ├── GameEngineTests.cs
    └── LevelConfigTests.cs
```

## 🛠️ Tech Stack

- **.NET 10** / **C# 13**
- **WinForms** — GUI & rendering (GDI+)
- **xUnit** — Unit testing
- No external NuGet packages required

## 🚀 Running the Project

```bash
# Build
dotnet build MazeRunner.App/MazeRunner.App.csproj

# Run
dotnet run --project MazeRunner.App/MazeRunner.App.csproj

# Tests
dotnet test MazeRunner.Tests/MazeRunner.Tests.csproj
```

## 🧪 Tests

```
Test summary: total: 56, failed: 0, succeeded: 56
```

Covers: maze generation correctness, player movement, wall collision, level config, and star calculation logic.

using MazeRunner.App.Rendering;
using MazeRunner.Core.Services;
using MazeRunner.Core.Session;

namespace MazeRunner.App.Forms;

public sealed class GameForm : Form
{
    // ── Core ─────────────────────────────────────────────────────────────────
    private readonly GameEngine _engine = new();
    private readonly int        _levelNumber;
    private readonly int        _cellSize;

    /// <summary>Set to the next level number if the player clicked "Next Level" on the result screen.</summary>
    public int? RequestedNextLevel { get; private set; }

    // ── Timing ───────────────────────────────────────────────────────────────
    private readonly System.Windows.Forms.Timer _gameTimer = new() { Interval = 1000 };
    private int _elapsedSeconds;

    // ── UI ───────────────────────────────────────────────────────────────────
    private MazePanel   _mazePanel = null!;
    private Label       _timerLabel = null!;
    private Label       _stepsLabel = null!;

    public GameForm(int levelNumber)
    {
        _levelNumber = levelNumber;
        _cellSize    = SpriteRenderer.GetCellSize(levelNumber);

        _engine.LoadLevel(levelNumber);
        InitializeComponent();

        _gameTimer.Tick += OnTimerTick;
        _gameTimer.Start();
    }

    private void InitializeComponent()
    {
        var settings  = LevelConfig.GetLevel(_levelNumber);
        int mazePixel = settings.GridSize * _cellSize;
        int formW     = Math.Max(660, mazePixel + 40);
        int formH     = mazePixel + 100;  // 100 = HUD(54) + margins

        Text            = $"Maze Runner — Level {_levelNumber}";
        ClientSize      = new Size(formW, formH);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.FromArgb(18, 18, 24);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        KeyPreview      = true;

        KeyDown += OnKeyDown;

        // ── HUD panel ────────────────────────────────────────────────────────
        var hudPanel = new Panel
        {
            Size      = new Size(formW, 54),
            Location  = new Point(0, 0),
            BackColor = Color.FromArgb(28, 28, 38)
        };

        var levelLabel = new Label
        {
            Text      = $"Level {_levelNumber}",
            Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            AutoSize  = true,
            Location  = new Point(14, 15)
        };

        _stepsLabel = new Label
        {
            Text      = "Steps: 0",
            Font      = new Font("Segoe UI", 11f),
            ForeColor = Color.FromArgb(200, 200, 200),
            AutoSize  = true,
            Location  = new Point(160, 17)
        };

        _timerLabel = new Label
        {
            Text      = "00:00",
            Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 200, 100),
            AutoSize  = true,
            Location  = new Point(330, 15)
        };

        var exitButton = new Button
        {
            Text        = "✕ EXIT",
            Font        = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor   = Color.White,
            BackColor   = Color.FromArgb(140, 40, 40),
            FlatStyle   = FlatStyle.Flat,
            Size        = new Size(90, 32),
            Location    = new Point(formW - 110, 11),
            Cursor      = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
        exitButton.Click += (_, _) => ExitToLevelSelect();

        hudPanel.Controls.AddRange([levelLabel, _stepsLabel, _timerLabel, exitButton]);

        // ── Maze panel ───────────────────────────────────────────────────────
        int mazeOffsetX = (formW - mazePixel) / 2;
        _mazePanel = new MazePanel(_engine, _cellSize)
        {
            Size     = new Size(mazePixel, mazePixel),
            Location = new Point(mazeOffsetX, 57)
        };

        Controls.AddRange([hudPanel, _mazePanel]);
    }

    // ── Timer ─────────────────────────────────────────────────────────────────
    private void OnTimerTick(object? sender, EventArgs e)
    {
        _elapsedSeconds++;
        UpdateHud();
    }

    // ── Keyboard input ────────────────────────────────────────────────────────
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_engine.IsCompleted) return;

        var dir = e.KeyCode switch
        {
            Keys.W or Keys.Up    => (MazeRunner.Core.Interfaces.Direction?)MazeRunner.Core.Interfaces.Direction.Up,
            Keys.S or Keys.Down  => MazeRunner.Core.Interfaces.Direction.Down,
            Keys.A or Keys.Left  => MazeRunner.Core.Interfaces.Direction.Left,
            Keys.D or Keys.Right => MazeRunner.Core.Interfaces.Direction.Right,
            _                    => (MazeRunner.Core.Interfaces.Direction?)null
        };

        if (dir is null) return;

        _engine.MovePlayer(dir.Value);
        UpdateHud();
        _mazePanel.Invalidate();

        if (_engine.IsCompleted)
            OnLevelCompleted();
    }

    // ── HUD update ────────────────────────────────────────────────────────────
    private void UpdateHud()
    {
        _stepsLabel.Text  = $"Steps: {_engine.Player.Steps}";
        int mins = _elapsedSeconds / 60;
        int secs = _elapsedSeconds % 60;
        _timerLabel.Text  = $"{mins:D2}:{secs:D2}";
    }

    // ── Level completion ──────────────────────────────────────────────────────
    private void OnLevelCompleted()
    {
        _gameTimer.Stop();

        int stars = LevelConfig.CalculateStars(_levelNumber, _elapsedSeconds);
        SessionData.UpdateStars(_levelNumber, stars);

        using var result = new ResultForm(_levelNumber, stars, _elapsedSeconds, _engine.Player.Steps);
        var dlgResult = result.ShowDialog(this);

        // If player clicked "Next Level", signal the caller to open the next level
        if (dlgResult == DialogResult.OK && _levelNumber < LevelConfig.TotalLevels)
            RequestedNextLevel = _levelNumber + 1;

        Close();
    }

    private void ExitToLevelSelect()
    {
        _gameTimer.Stop();
        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _gameTimer.Dispose();
        base.OnFormClosed(e);
    }
}

// ── Double-buffered maze panel ────────────────────────────────────────────────
internal sealed class MazePanel : Panel
{
    private readonly GameEngine _engine;
    private readonly int        _cellSize;

    public MazePanel(GameEngine engine, int cellSize)
    {
        _engine   = engine;
        _cellSize = cellSize;

        DoubleBuffered = true;
        BackColor      = Color.FromArgb(12, 12, 16);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g     = e.Graphics;
        var level = _engine.CurrentLevel;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Draw maze cells
        for (int row = 0; row < level.Height; row++)
        {
            for (int col = 0; col < level.Width; col++)
            {
                int px = col * _cellSize;
                int py = row * _cellSize;
                SpriteRenderer.DrawCell(g, level.Grid[row, col], px, py, _cellSize);
            }
        }

        // Draw player on top
        int playerPx = _engine.Player.X * _cellSize;
        int playerPy = _engine.Player.Y * _cellSize;
        SpriteRenderer.DrawPlayer(g, playerPx, playerPy, _cellSize);
    }
}

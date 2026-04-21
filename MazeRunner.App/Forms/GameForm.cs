using MazeRunner.App.Rendering;
using MazeRunner.Core.Services;
using MazeRunner.Core.Session;

namespace MazeRunner.App.Forms;

public sealed class GameForm : Form
{
    private readonly GameEngine _engine = new();
    private readonly int        _levelNumber;
    private int                 _cellSize;

    public int? RequestedNextLevel { get; private set; }

    private readonly System.Windows.Forms.Timer _gameTimer = new() { Interval = 1000 };
    private int _elapsedSeconds;

    private MazePanel _mazePanel    = null!;
    private Label     _timerLabel   = null!;
    private Label     _stepsLabel   = null!;
    private Label     _keyLabel     = null!;
    private Label     _hintLabel    = null!;

    private readonly System.Windows.Forms.Timer _hintTimer = new() { Interval = 1800 };

    public GameForm(int levelNumber)
    {
        _levelNumber = levelNumber;
        _engine.LoadLevel(levelNumber);
        InitializeComponent();

        _gameTimer.Tick += OnTimerTick;
        _gameTimer.Start();

        _hintTimer.Tick += (_, _) => { _hintLabel.Visible = false; _hintTimer.Stop(); };
    }

    private void InitializeComponent()
    {
        var screen    = Screen.PrimaryScreen!.WorkingArea;
        int hudHeight = 54;
        int padding   = 10;

        var settings  = LevelConfig.GetLevel(_levelNumber);
        int gridSize  = settings.GridSize;

        int availW = screen.Width  - padding * 2;
        int availH = screen.Height - hudHeight - padding * 2;

        _cellSize = SpriteRenderer.ComputeCellSize(gridSize, availW, availH);

        int mazePixelW = gridSize * _cellSize;
        int mazePixelH = gridSize * _cellSize;

        Text            = $"Maze Runner — Level {_levelNumber}";
        FormBorderStyle = FormBorderStyle.None;
        WindowState     = FormWindowState.Maximized;
        BackColor       = Color.FromArgb(18, 18, 24);
        KeyPreview      = true;

        KeyDown += OnKeyDown;

        int formW = screen.Width;

        var hudPanel = new Panel
        {
            Size      = new Size(formW, hudHeight),
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

        _keyLabel = new Label
        {
            Text      = "🔑 Key: NOT FOUND",
            Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Color.FromArgb(200, 150, 50),
            AutoSize  = true,
            Location  = new Point(490, 17)
        };

        _hintLabel = new Label
        {
            Text      = "🔒 Find the key first!",
            Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 80, 80),
            AutoSize  = true,
            Location  = new Point(700, 17),
            Visible   = false
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

        hudPanel.Controls.AddRange([levelLabel, _stepsLabel, _timerLabel,
                                    _keyLabel, _hintLabel, exitButton]);

        int mazeOffsetX = (screen.Width  - mazePixelW) / 2;
        int mazeOffsetY = hudHeight + (screen.Height - hudHeight - mazePixelH) / 2;

        _mazePanel = new MazePanel(_engine, _cellSize)
        {
            Size     = new Size(mazePixelW, mazePixelH),
            Location = new Point(mazeOffsetX, mazeOffsetY)
        };

        Controls.AddRange([hudPanel, _mazePanel]);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _elapsedSeconds++;
        UpdateHud();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_engine.IsCompleted) return;

        var dir = e.KeyCode switch
        {
            Keys.W or Keys.Up    => (MazeRunner.Core.Interfaces.Direction?)MazeRunner.Core.Interfaces.Direction.Up,
            Keys.S or Keys.Down  => MazeRunner.Core.Interfaces.Direction.Down,
            Keys.A or Keys.Left  => MazeRunner.Core.Interfaces.Direction.Left,
            Keys.D or Keys.Right => MazeRunner.Core.Interfaces.Direction.Right,
            Keys.Escape          => null,
            _                    => (MazeRunner.Core.Interfaces.Direction?)null
        };

        if (e.KeyCode == Keys.Escape) { ExitToLevelSelect(); return; }
        if (dir is null) return;

        _engine.MovePlayer(dir.Value);
        UpdateHud();

        if (_engine.ExitBlockedFeedback)
        {
            _hintLabel.Visible = true;
            _hintTimer.Stop();
            _hintTimer.Start();
        }

        _mazePanel.Invalidate();

        if (_engine.IsCompleted)
            OnLevelCompleted();
    }

    private void UpdateHud()
    {
        _stepsLabel.Text = $"Steps: {_engine.Player.Steps}";
        int mins = _elapsedSeconds / 60;
        int secs = _elapsedSeconds % 60;
        _timerLabel.Text = $"{mins:D2}:{secs:D2}";

        if (_engine.HasKey)
        {
            _keyLabel.Text      = "🔑 Key: FOUND ✓";
            _keyLabel.ForeColor = Color.FromArgb(100, 220, 100);
        }
    }

    private void OnLevelCompleted()
    {
        _gameTimer.Stop();
        _hintTimer.Stop();

        int stars = LevelConfig.CalculateStars(_levelNumber, _elapsedSeconds);
        SessionData.UpdateStars(_levelNumber, stars);

        using var result = new ResultForm(_levelNumber, stars, _elapsedSeconds, _engine.Player.Steps);
        var dlgResult = result.ShowDialog(this);

        if (dlgResult == DialogResult.OK && _levelNumber < LevelConfig.TotalLevels)
            RequestedNextLevel = _levelNumber + 1;

        Close();
    }

    private void ExitToLevelSelect()
    {
        _gameTimer.Stop();
        _hintTimer.Stop();
        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _gameTimer.Dispose();
        _hintTimer.Dispose();
        base.OnFormClosed(e);
    }
}

internal sealed class MazePanel : Panel
{
    private readonly GameEngine _engine;
    private readonly int        _cellSize;

    public MazePanel(GameEngine engine, int cellSize)
    {
        _engine        = engine;
        _cellSize      = cellSize;
        DoubleBuffered = true;
        BackColor      = Color.FromArgb(12, 12, 16);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g     = e.Graphics;
        var level = _engine.CurrentLevel;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        for (int row = 0; row < level.Height; row++)
        {
            for (int col = 0; col < level.Width; col++)
            {
                int px = col * _cellSize;
                int py = row * _cellSize;
                SpriteRenderer.DrawCell(g, level.Grid[row, col], px, py, _cellSize);
            }
        }

        int playerPx = _engine.Player.X * _cellSize;
        int playerPy = _engine.Player.Y * _cellSize;
        SpriteRenderer.DrawPlayer(g, playerPx, playerPy, _cellSize);
    }
}

namespace MazeRunner.App.Forms;

public sealed class ResultForm : Form
{
    private readonly int _levelNumber;
    private readonly int _stars;
    private readonly int _elapsedSeconds;
    private readonly int _steps;

    public ResultForm(int levelNumber, int stars, int elapsedSeconds, int steps)
    {
        _levelNumber    = levelNumber;
        _stars          = stars;
        _elapsedSeconds = elapsedSeconds;
        _steps          = steps;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "Level Complete!";
        Size            = new Size(420, 380);
        MinimumSize     = new Size(420, 380);
        MaximumSize     = new Size(420, 380);
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Color.FromArgb(22, 22, 30);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;

        // ── Header ────────────────────────────────────────────────────────────
        var headerLabel = new Label
        {
            Text      = "🏆  LEVEL COMPLETE!",
            Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(380, 50),
            Location  = new Point(20, 20)
        };

        // ── Stars display ─────────────────────────────────────────────────────
        string starText = _stars switch
        {
            3 => "★★★",
            2 => "★★☆",
            1 => "★☆☆",
            _ => "☆☆☆"
        };

        var starsLabel = new Label
        {
            Text      = starText,
            Font      = new Font("Segoe UI", 40f),
            ForeColor = Color.FromArgb(255, 215, 0),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(380, 80),
            Location  = new Point(20, 75)
        };

        // ── Star description ──────────────────────────────────────────────────
        string desc = _stars switch
        {
            3 => "⚡ Blazing fast!",
            2 => "👍 Good job!",
            1 => "🐢 Keep practicing!",
            _ => ""
        };

        var descLabel = new Label
        {
            Text      = desc,
            Font      = new Font("Segoe UI", 12f, FontStyle.Italic),
            ForeColor = Color.FromArgb(180, 180, 180),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(380, 28),
            Location  = new Point(20, 155)
        };

        // ── Stats ─────────────────────────────────────────────────────────────
        int mins = _elapsedSeconds / 60;
        int secs = _elapsedSeconds % 60;

        var statsLabel = new Label
        {
            Text      = $"Time: {mins:D2}:{secs:D2}     Steps: {_steps}",
            Font      = new Font("Segoe UI", 11f),
            ForeColor = Color.FromArgb(140, 200, 140),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(380, 28),
            Location  = new Point(20, 190)
        };

        // ── Buttons ───────────────────────────────────────────────────────────
        var levelSelectButton = new Button
        {
            Text        = "◀  Level Select",
            Font        = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor   = Color.White,
            BackColor   = Color.FromArgb(70, 70, 90),
            FlatStyle   = FlatStyle.Flat,
            Size        = new Size(160, 46),
            Location    = new Point(30, 275),
            Cursor      = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
        levelSelectButton.Click += (_, _) => Close();

        var nextButton = new Button
        {
            Text        = "Next Level  ▶",
            Font        = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor   = Color.White,
            BackColor   = Color.FromArgb(60, 130, 220),
            FlatStyle   = FlatStyle.Flat,
            Size        = new Size(160, 46),
            Location    = new Point(230, 275),
            Cursor      = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 },
            Enabled     = _levelNumber < 6
        };

        if (_levelNumber >= 6)
        {
            // Final level completed — change Next button to "You Win!"
            nextButton.Text      = "🎉 You Won!";
            nextButton.BackColor = Color.FromArgb(60, 160, 60);
            nextButton.Enabled   = true;
            nextButton.Click    += (_, _) => Close();
        }
        else
        {
            nextButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        Controls.AddRange([headerLabel, starsLabel, descLabel, statsLabel,
                           levelSelectButton, nextButton]);
    }
}

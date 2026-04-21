using MazeRunner.Core.Session;

namespace MazeRunner.App.Forms;

public sealed class MainMenuForm : Form
{
    public MainMenuForm()
    {
        InitializeComponent();
        SessionData.Reset();
    }

    private void InitializeComponent()
    {
        Text            = "Maze Runner";
        Size            = new Size(480, 520);
        MinimumSize     = new Size(480, 520);
        MaximumSize     = new Size(480, 520);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.FromArgb(18, 18, 24);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;

        // ── Title label ──────────────────────────────────────────────────────
        var titleLabel = new Label
        {
            Text      = "MAZE RUNNER",
            Font      = new Font("Segoe UI", 32f, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(440, 80),
            Location  = new Point(20, 80)
        };

        var subtitleLabel = new Label
        {
            Text      = "Can you find the exit?",
            Font      = new Font("Segoe UI", 12f, FontStyle.Italic),
            ForeColor = Color.FromArgb(160, 160, 160),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(440, 30),
            Location  = new Point(20, 165)
        };

        // ── Buttons ──────────────────────────────────────────────────────────
        var playButton = CreateMenuButton("▶  PLAY", new Point(140, 250));
        playButton.Click += (_, _) =>
        {
            Hide();
            using var levelForm = new LevelSelectForm();
            levelForm.ShowDialog(this);
            Show();
        };

        var exitButton = CreateMenuButton("✕  EXIT", new Point(140, 340));
        exitButton.BackColor = Color.FromArgb(120, 30, 30);
        exitButton.Click += (_, _) => Application.Exit();

        // ── Decorative maze icon (Unicode) ───────────────────────────────────
        var iconLabel = new Label
        {
            Text      = "⬛",
            Font      = new Font("Segoe UI Emoji", 48f),
            ForeColor = Color.FromArgb(60, 130, 220),
            AutoSize  = true,
            Location  = new Point(210, 10)
        };

        Controls.AddRange([iconLabel, titleLabel, subtitleLabel, playButton, exitButton]);
    }

    private static Button CreateMenuButton(string text, Point location)
    {
        return new Button
        {
            Text        = text,
            Font        = new Font("Segoe UI", 14f, FontStyle.Bold),
            ForeColor   = Color.White,
            BackColor   = Color.FromArgb(60, 130, 220),
            FlatStyle   = FlatStyle.Flat,
            Size        = new Size(200, 55),
            Location    = location,
            Cursor      = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
    }
}

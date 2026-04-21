using MazeRunner.Core.Services;
using MazeRunner.Core.Session;

namespace MazeRunner.App.Forms;

public sealed class LevelSelectForm : Form
{
    private readonly Panel[] _levelCards = new Panel[LevelConfig.TotalLevels];

    public LevelSelectForm()
    {
        InitializeComponent();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RefreshStars();
    }

    private void InitializeComponent()
    {
        Text            = "Maze Runner — Select Level";
        Size            = new Size(620, 560);
        MinimumSize     = new Size(620, 560);
        MaximumSize     = new Size(620, 560);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.FromArgb(18, 18, 24);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;

        var titleLabel = new Label
        {
            Text      = "SELECT LEVEL",
            Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(580, 50),
            Location  = new Point(20, 15)
        };
        Controls.Add(titleLabel);

        int cardW  = 160;
        int cardH  = 130;
        int startX = 30;
        int startY = 80;
        int padX   = 20;
        int padY   = 20;

        for (int i = 0; i < LevelConfig.TotalLevels; i++)
        {
            int levelNum = i + 1;
            int col = i % 3;
            int row = i / 3;
            int x   = startX + col * (cardW + padX);
            int y   = startY + row * (cardH + padY);

            var card = CreateLevelCard(levelNum, x, y, cardW, cardH);
            _levelCards[i] = card;
            Controls.Add(card);
        }

        var backButton = new Button
        {
            Text        = "◀  BACK",
            Font        = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor   = Color.White,
            BackColor   = Color.FromArgb(70, 70, 90),
            FlatStyle   = FlatStyle.Flat,
            Size        = new Size(140, 44),
            Location    = new Point(230, 460),
            Cursor      = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
        backButton.Click += (_, _) => Close();
        Controls.Add(backButton);
    }

    private Panel CreateLevelCard(int levelNum, int x, int y, int w, int h)
    {
        var card = new Panel
        {
            Size      = new Size(w, h),
            Location  = new Point(x, y),
            BackColor = Color.FromArgb(30, 30, 40),
            Cursor    = Cursors.Hand
        };

        var numLabel = new Label
        {
            Text      = $"Level {levelNum}",
            Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(w, 36),
            Location  = new Point(0, 12),
            Tag       = $"title_{levelNum}"
        };

        var starsLabel = new Label
        {
            Text      = "☆☆☆",
            Font      = new Font("Segoe UI", 18f),
            ForeColor = Color.FromArgb(180, 160, 0),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(w, 40),
            Location  = new Point(0, 50),
            Tag       = $"stars_{levelNum}"
        };

        var settings  = LevelConfig.GetLevel(levelNum);
        var sizeLabel = new Label
        {
            Text      = $"{settings.GridSize}×{settings.GridSize}",
            Font      = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(120, 120, 140),
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size      = new Size(w, 22),
            Location  = new Point(0, 95)
        };

        card.Controls.AddRange([numLabel, starsLabel, sizeLabel]);

        card.Click += (_, _) => LaunchLevel(levelNum);
        foreach (Control child in card.Controls)
            child.Click += (_, _) => LaunchLevel(levelNum);

        card.MouseEnter += (_, _) => card.BackColor = Color.FromArgb(50, 50, 70);
        card.MouseLeave += (_, _) => card.BackColor = Color.FromArgb(30, 30, 40);
        foreach (Control child in card.Controls)
        {
            child.MouseEnter += (_, _) => card.BackColor = Color.FromArgb(50, 50, 70);
            child.MouseLeave += (_, _) => card.BackColor = Color.FromArgb(30, 30, 40);
        }

        return card;
    }

    private void LaunchLevel(int levelNum)
    {
        int? nextLevel = levelNum;

        while (nextLevel.HasValue)
        {
            using var gameForm = new GameForm(nextLevel.Value);
            gameForm.ShowDialog(this);
            nextLevel = gameForm.RequestedNextLevel;
        }

        RefreshStars();
    }

    private void RefreshStars()
    {
        for (int i = 0; i < LevelConfig.TotalLevels; i++)
        {
            int levelNum = i + 1;
            int best     = SessionData.GetBestStars(levelNum);

            foreach (Control ctrl in _levelCards[i].Controls)
            {
                if (ctrl.Tag is string tag && tag == $"stars_{levelNum}")
                {
                    ctrl.Text = BuildStarString(best);
                    break;
                }
            }
        }
    }

    private static string BuildStarString(int filled) => filled switch
    {
        1 => "★☆☆",
        2 => "★★☆",
        3 => "★★★",
        _ => "☆☆☆"
    };
}

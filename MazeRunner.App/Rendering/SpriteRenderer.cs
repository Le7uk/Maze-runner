using MazeRunner.Core.Models;

namespace MazeRunner.App.Rendering;

/// <summary>
/// Handles all GDI+ drawing for the maze cells and player character.
/// All sprites are drawn programmatically — no external image files required.
/// </summary>
public static class SpriteRenderer
{
    // Colours
    private static readonly Color WallColor      = Color.FromArgb(45,  45,  48);
    private static readonly Color WallEdgeLight  = Color.FromArgb(80,  80,  85);
    private static readonly Color WallEdgeDark   = Color.FromArgb(20,  20,  22);
    private static readonly Color FloorColor     = Color.FromArgb(200, 190, 170);
    private static readonly Color FloorLine      = Color.FromArgb(180, 170, 150);
    private static readonly Color StartColor     = Color.FromArgb(80,  180, 100);
    private static readonly Color ExitColor      = Color.FromArgb(255, 215, 0);
    private static readonly Color ExitGlow       = Color.FromArgb(255, 180, 0);

    // Player colours
    private static readonly Color PlayerBody     = Color.FromArgb(60,  130, 220);
    private static readonly Color PlayerOutline  = Color.FromArgb(20,  70,  160);
    private static readonly Color PlayerEye      = Color.White;
    private static readonly Color PlayerPupil    = Color.FromArgb(20,  20,  80);

    /// <summary>Returns the pixel size of each cell for a given level number.</summary>
    public static int GetCellSize(int levelNumber) => levelNumber switch
    {
        1 => 48,
        2 => 40,
        3 => 36,
        4 => 32,
        5 => 28,
        6 => 24,
        _ => 32
    };

    /// <summary>Draws a single maze cell at the given pixel coordinates.</summary>
    public static void DrawCell(Graphics g, Cell cell, int px, int py, int size)
    {
        switch (cell)
        {
            case Cell.Wall:  DrawWall(g, px, py, size);  break;
            case Cell.Floor: DrawFloor(g, px, py, size); break;
            case Cell.Start: DrawStart(g, px, py, size); break;
            case Cell.Exit:  DrawExit(g, px, py, size);  break;
        }
    }

    /// <summary>Draws the player character (top-down view) at the given pixel coordinates.</summary>
    public static void DrawPlayer(Graphics g, int px, int py, int size)
    {
        int margin = Math.Max(2, size / 8);
        var bodyRect = new Rectangle(px + margin, py + margin, size - margin * 2, size - margin * 2);

        // Body circle
        using var bodyBrush = new SolidBrush(PlayerBody);
        g.FillEllipse(bodyBrush, bodyRect);

        // Outline
        using var outline = new Pen(PlayerOutline, Math.Max(1f, size / 16f));
        g.DrawEllipse(outline, bodyRect);

        if (size >= 20)
        {
            // Left eye
            int eyeSize = Math.Max(2, size / 7);
            int eyeY    = py + size / 3;
            int leftEyeX  = px + size / 3 - eyeSize / 2;
            int rightEyeX = px + size * 2 / 3 - eyeSize / 2;

            using var eyeBrush = new SolidBrush(PlayerEye);
            g.FillEllipse(eyeBrush, leftEyeX,  eyeY, eyeSize, eyeSize);
            g.FillEllipse(eyeBrush, rightEyeX, eyeY, eyeSize, eyeSize);

            // Pupils
            int pupilSize = Math.Max(1, eyeSize / 2);
            int offsetX   = eyeSize / 4;
            using var pupilBrush = new SolidBrush(PlayerPupil);
            g.FillEllipse(pupilBrush, leftEyeX  + offsetX, eyeY + offsetX, pupilSize, pupilSize);
            g.FillEllipse(pupilBrush, rightEyeX + offsetX, eyeY + offsetX, pupilSize, pupilSize);
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static void DrawWall(Graphics g, int px, int py, int size)
    {
        using var brush = new SolidBrush(WallColor);
        g.FillRectangle(brush, px, py, size, size);

        // Bevel effect — lighter top-left, darker bottom-right
        using var lightPen = new Pen(WallEdgeLight, 1);
        using var darkPen  = new Pen(WallEdgeDark,  1);

        g.DrawLine(lightPen, px,          py,          px + size - 1, py);          // top
        g.DrawLine(lightPen, px,          py,          px,            py + size - 1); // left
        g.DrawLine(darkPen,  px + size-1, py,          px + size - 1, py + size - 1); // right
        g.DrawLine(darkPen,  px,          py + size-1, px + size - 1, py + size - 1); // bottom
    }

    private static void DrawFloor(Graphics g, int px, int py, int size)
    {
        using var brush = new SolidBrush(FloorColor);
        g.FillRectangle(brush, px, py, size, size);

        // Subtle grid line on edges
        using var pen = new Pen(FloorLine, 1);
        g.DrawRectangle(pen, px, py, size - 1, size - 1);
    }

    private static void DrawStart(Graphics g, int px, int py, int size)
    {
        // Floor base
        DrawFloor(g, px, py, size);

        // Green tint overlay
        using var tint = new SolidBrush(Color.FromArgb(60, 80, 180, 100));
        g.FillRectangle(tint, px + 1, py + 1, size - 2, size - 2);

        if (size >= 20)
        {
            // Draw a small "S" indicator
            using var font = new Font("Arial", Math.Max(6f, size * 0.3f), FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.FromArgb(30, 120, 50));
            var textRect = new RectangleF(px, py, size, size);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString("S", font, textBrush, textRect, format);
        }
    }

    private static void DrawExit(Graphics g, int px, int py, int size)
    {
        // Floor base
        DrawFloor(g, px, py, size);

        // Gold overlay
        using var gold = new SolidBrush(Color.FromArgb(180, 255, 215, 0));
        g.FillRectangle(gold, px + 1, py + 1, size - 2, size - 2);

        if (size >= 20)
        {
            // Draw a star symbol
            DrawStar(g, px + size / 2, py + size / 2, size * 0.35f, ExitGlow);
        }
    }

    private static void DrawStar(Graphics g, int cx, int cy, float radius, Color color)
    {
        int points = 5;
        var outer = new PointF[points];
        var inner = new PointF[points];
        float innerRadius = radius * 0.45f;

        for (int i = 0; i < points; i++)
        {
            double outerAngle = Math.PI * (i * 2 - 0.5) / points;
            double innerAngle = Math.PI * (i * 2 + 0.5) / points;
            outer[i] = new PointF(cx + (float)(radius * Math.Cos(outerAngle)),
                                  cy + (float)(radius * Math.Sin(outerAngle)));
            inner[i] = new PointF(cx + (float)(innerRadius * Math.Cos(innerAngle)),
                                  cy + (float)(innerRadius * Math.Sin(innerAngle)));
        }

        var star = new PointF[points * 2];
        for (int i = 0; i < points; i++)
        {
            star[i * 2]     = outer[i];
            star[i * 2 + 1] = inner[i];
        }

        using var brush = new SolidBrush(color);
        g.FillPolygon(brush, star);
    }
}

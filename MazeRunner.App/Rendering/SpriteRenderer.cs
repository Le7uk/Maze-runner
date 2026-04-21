using MazeRunner.Core.Models;

namespace MazeRunner.App.Rendering;

public static class SpriteRenderer
{
    private static readonly Color WallColor     = Color.FromArgb(45,  45,  48);
    private static readonly Color WallEdgeLight = Color.FromArgb(80,  80,  85);
    private static readonly Color WallEdgeDark  = Color.FromArgb(20,  20,  22);
    private static readonly Color FloorColor    = Color.FromArgb(200, 190, 170);
    private static readonly Color FloorLine     = Color.FromArgb(180, 170, 150);

    private static readonly Color DoorWood      = Color.FromArgb(139, 90,  43);
    private static readonly Color DoorDark      = Color.FromArgb(90,  55,  20);
    private static readonly Color DoorLight     = Color.FromArgb(170, 115, 60);
    private static readonly Color DoorKnob      = Color.FromArgb(255, 215, 0);

    private static readonly Color PlayerBody    = Color.FromArgb(60,  130, 220);
    private static readonly Color PlayerOutline = Color.FromArgb(20,  70,  160);
    private static readonly Color PlayerEye     = Color.White;
    private static readonly Color PlayerPupil   = Color.FromArgb(20,  20,  80);

    public static int ComputeCellSize(int gridSize, int availableWidth, int availableHeight)
    {
        int byWidth  = availableWidth  / gridSize;
        int byHeight = availableHeight / gridSize;
        return Math.Max(Math.Min(byWidth, byHeight), 10);
    }

    public static void DrawCell(Graphics g, Cell cell, int px, int py, int size)
    {
        switch (cell)
        {
            case Cell.Wall:  DrawWall(g, px, py, size);    break;
            case Cell.Floor: DrawFloor(g, px, py, size);   break;
            case Cell.Start: DrawStart(g, px, py, size);   break;
            case Cell.Exit:  DrawDoor(g, px, py, size);    break;
            case Cell.Key:   DrawKeyCell(g, px, py, size); break;
        }
    }

    public static void DrawPlayer(Graphics g, int px, int py, int size)
    {
        int margin   = Math.Max(2, size / 8);
        var bodyRect = new Rectangle(px + margin, py + margin, size - margin * 2, size - margin * 2);

        using var bodyBrush = new SolidBrush(PlayerBody);
        g.FillEllipse(bodyBrush, bodyRect);

        using var outline = new Pen(PlayerOutline, Math.Max(1f, size / 16f));
        g.DrawEllipse(outline, bodyRect);

        if (size >= 20)
        {
            int eyeSize   = Math.Max(2, size / 7);
            int eyeY      = py + size / 3;
            int leftEyeX  = px + size / 3 - eyeSize / 2;
            int rightEyeX = px + size * 2 / 3 - eyeSize / 2;

            using var eyeBrush = new SolidBrush(PlayerEye);
            g.FillEllipse(eyeBrush, leftEyeX,  eyeY, eyeSize, eyeSize);
            g.FillEllipse(eyeBrush, rightEyeX, eyeY, eyeSize, eyeSize);

            int pupilSize = Math.Max(1, eyeSize / 2);
            int offsetX   = eyeSize / 4;
            using var pupilBrush = new SolidBrush(PlayerPupil);
            g.FillEllipse(pupilBrush, leftEyeX  + offsetX, eyeY + offsetX, pupilSize, pupilSize);
            g.FillEllipse(pupilBrush, rightEyeX + offsetX, eyeY + offsetX, pupilSize, pupilSize);
        }
    }

    private static void DrawWall(Graphics g, int px, int py, int size)
    {
        using var brush    = new SolidBrush(WallColor);
        using var lightPen = new Pen(WallEdgeLight, 1);
        using var darkPen  = new Pen(WallEdgeDark,  1);

        g.FillRectangle(brush, px, py, size, size);
        g.DrawLine(lightPen, px,          py,          px + size - 1, py);
        g.DrawLine(lightPen, px,          py,          px,            py + size - 1);
        g.DrawLine(darkPen,  px + size-1, py,          px + size - 1, py + size - 1);
        g.DrawLine(darkPen,  px,          py + size-1, px + size - 1, py + size - 1);
    }

    private static void DrawFloor(Graphics g, int px, int py, int size)
    {
        using var brush = new SolidBrush(FloorColor);
        using var pen   = new Pen(FloorLine, 1);
        g.FillRectangle(brush, px, py, size, size);
        g.DrawRectangle(pen,   px, py, size - 1, size - 1);
    }

    private static void DrawStart(Graphics g, int px, int py, int size)
    {
        DrawFloor(g, px, py, size);
        using var tint = new SolidBrush(Color.FromArgb(60, 80, 180, 100));
        g.FillRectangle(tint, px + 1, py + 1, size - 2, size - 2);

        if (size >= 20)
        {
            using var font      = new Font("Arial", Math.Max(6f, size * 0.3f), FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.FromArgb(30, 120, 50));
            var format = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString("S", font, textBrush, new RectangleF(px, py, size, size), format);
        }
    }

    private static void DrawDoor(Graphics g, int px, int py, int size)
    {
        DrawFloor(g, px, py, size);

        int m  = Math.Max(1, size / 8);
        int dw = size - m * 2;
        int dh = size - m;

        int dx = px + m;
        int dy = py + m / 2;

        using var frameBrush = new SolidBrush(DoorDark);
        using var doorBrush  = new SolidBrush(DoorWood);
        using var lightBrush = new SolidBrush(DoorLight);
        using var knobBrush  = new SolidBrush(DoorKnob);
        using var framePen   = new Pen(DoorDark, Math.Max(1f, size / 14f));

        g.FillRectangle(frameBrush, dx, dy, dw, dh);
        int border = Math.Max(1, size / 10);
        g.FillRectangle(doorBrush, dx + border, dy + border, dw - border * 2, dh - border);

        if (size >= 24)
        {
            int panelM = border + Math.Max(1, size / 12);
            int panelH = (dh - border * 2) / 2 - panelM / 2;
            int panelW = dw - border * 2 - panelM * 2;

            g.FillRectangle(lightBrush, dx + border + panelM, dy + border + panelM / 2, panelW, panelH);
            g.FillRectangle(lightBrush, dx + border + panelM, dy + border + panelM / 2 + panelH + panelM / 2, panelW, panelH);
        }

        int knobR  = Math.Max(2, size / 9);
        int knobX  = dx + dw - border - knobR * 2;
        int knobY  = dy + dh / 2 - knobR;
        g.FillEllipse(knobBrush, knobX, knobY, knobR * 2, knobR * 2);
        g.DrawEllipse(framePen,  knobX, knobY, knobR * 2, knobR * 2);
    }

    private static void DrawKeyCell(Graphics g, int px, int py, int size)
    {
        DrawFloor(g, px, py, size);
        if (size < 14) return;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        float s = size;

        float bowCX = px + s * 0.27f;
        float bowCY = py + s * 0.50f;
        float bowR  = s * 0.21f;
        float holeR = s * 0.09f;

        float sX1 = px + s * 0.38f;
        float sY1 = py + s * 0.42f;
        float sW  = s * 0.54f;
        float sH  = s * 0.16f;

        float t1X = px + s * 0.78f;
        float t1Y = py + s * 0.58f;
        float tW  = s * 0.09f;
        float t1H = s * 0.16f;

        float t2X = px + s * 0.64f;
        float t2H = s * 0.12f;

        using var goldBrush = new SolidBrush(Color.FromArgb(255, 200, 0));
        using var holeBrush = new SolidBrush(FloorColor);
        using var darkPen   = new Pen(Color.FromArgb(120, 75, 0), Math.Max(1f, s / 20f));

        g.FillRectangle(goldBrush, sX1, sY1, sW, sH);
        g.FillRectangle(goldBrush, t1X, t1Y, tW, t1H);
        g.FillRectangle(goldBrush, t2X, t1Y, tW, t2H);

        g.FillEllipse(goldBrush, bowCX - bowR, bowCY - bowR, bowR * 2, bowR * 2);
        g.FillEllipse(holeBrush, bowCX - holeR, bowCY - holeR, holeR * 2, holeR * 2);
        g.DrawEllipse(darkPen,   bowCX - bowR, bowCY - bowR, bowR * 2, bowR * 2);
    }
}

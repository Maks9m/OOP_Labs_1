using Avalonia;
using Avalonia.Media;
using System;

namespace Lab4.Shapes;

// Каркас куба - множинне успадкування через композицію
public sealed class CubeShape : ShapeBase
{
    private RectShape _frontRect;
    private RectShape _backRect;
    private LineShape[] _connectionLines;

    public CubeShape(Point topLeft, Point bottomRight) : base(topLeft, bottomRight)
    {
        var rect = new Rect(topLeft, bottomRight).Normalize();
        var depth = Math.Min(rect.Width, rect.Height) / 3;
        
        // Передній прямокутник
        _frontRect = new RectShape(topLeft, bottomRight) { Fill = null };
        
        // Задній прямокутник (зміщений)
        var backTopLeft = new Point(rect.X + depth, rect.Y - depth);
        var backBottomRight = new Point(rect.Right + depth, rect.Bottom - depth);
        _backRect = new RectShape(backTopLeft, backBottomRight) { Fill = null };
        
        // З'єднувальні лінії
        _connectionLines = new LineShape[4];
        UpdateConnectionLines(rect, depth);
    }

    public override void Set(long x1, long y1, long x2, long y2)
    {
        var rect = new Rect(new Point(x1, y1), new Point(x2, y2)).Normalize();
        var depth = Math.Min(rect.Width, rect.Height) / 3;
        
        _frontRect.Set(x1, y1, x2, y2);
        _backRect.Set((long)(rect.X + depth), (long)(rect.Y - depth), 
                     (long)(rect.Right + depth), (long)(rect.Bottom - depth));
        
        UpdateConnectionLines(rect, depth);
    }

    private void UpdateConnectionLines(Rect frontRect, double depth)
    {
        // Лінії, що з'єднують передній і задній прямокутники
        _connectionLines[0] = new LineShape(
            frontRect.Left, frontRect.Top,
            frontRect.Left + depth, frontRect.Top - depth
        );
        _connectionLines[1] = new LineShape(
            frontRect.Right, frontRect.Top,
            frontRect.Right + depth, frontRect.Top - depth
        );
        _connectionLines[2] = new LineShape(
            frontRect.Left, frontRect.Bottom,
            frontRect.Left + depth, frontRect.Bottom - depth
        );
        _connectionLines[3] = new LineShape(
            frontRect.Right, frontRect.Bottom,
            frontRect.Right + depth, frontRect.Bottom - depth
        );
    }

    public override void Render(DrawingContext context)
    {
        // Використовуємо методи Show базових класів (поліморфізм)
        _frontRect.Render(context);
        _backRect.Render(context);
        
        foreach (var line in _connectionLines)
        {
            line.Render(context);
        }
    }
}
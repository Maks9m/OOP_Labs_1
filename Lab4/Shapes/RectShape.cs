using Avalonia;
using Avalonia.Media;

namespace Lab4.Shapes;

public sealed class RectShape : ShapeBase
{
    public IBrush? Fill { get; init; }
    public Pen Pen { get; init; } = new Pen(Brushes.Black, 1);

    public RectShape(Point p1, Point p2) : base(p1, p2) { }

    public override void Render(DrawingContext ctx)
    {
        var rect = new Rect(P1, P2).Normalize();
        ctx.DrawRectangle(Fill, Pen, rect);
    }

    public override ShapeBase CreateInstance(Point startPoint)
    {
        return new RectShape(startPoint, startPoint) { Fill = Brushes.Gray };
    }

    public override void PaintRubberBand(DrawingContext ctx, Pen pen, Point startPoint, Point currentPoint)
    {
        var rectFromCenter = BuildRectFromCenter(startPoint, currentPoint);
        ctx.DrawRectangle(null, pen, rectFromCenter);
    }

    public override void CalculateFinalCoordinates(Point startPoint, Point endPoint)
    {
        // Варіант 17: прямокутник від центру до кута
        var rect = BuildRectFromCenter(startPoint, endPoint);
        Set((long)rect.TopLeft.X, (long)rect.TopLeft.Y, (long)rect.BottomRight.X, (long)rect.BottomRight.Y);
    }

    private static Rect BuildRectFromCenter(Point center, Point corner)
    {
        var centerX = center.X;
        var centerY = center.Y;
        var cornerX = corner.X;
        var cornerY = corner.Y;
        
        var x1 = 2 * centerX - cornerX;
        var y1 = 2 * centerY - cornerY;
        
        return new Rect(new Point(x1, y1), new Point(cornerX, cornerY)).Normalize();
    }
}
